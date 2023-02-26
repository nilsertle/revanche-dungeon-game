#nullable enable
using System.Collections.Generic;
using Revanche.Extensions;
using Revanche.GameObjects;
using Revanche.GameObjects.FriendlyUnits;
using Revanche.GameObjects.HostileUnits;
using Revanche.GameObjects.Projectiles;
using Revanche.Managers;
using Revanche.Map.Pathfinding;
using Revanche.Sound;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Revanche.Core;

public sealed class CombatHandler
{
    private readonly LevelState mLevelState;
    private readonly IPathfinder mPathfinder;

    private Dictionary<string, string> AttackerToTarget => mLevelState.AttackerToTarget;
    private Dictionary<string, HashSet<string>> Attackers => mLevelState.Attackers;
    private readonly EventDispatcher mEventDispatcher;

    public CombatHandler(LevelState levelState, IPathfinder pathfinder, EventDispatcher eventDispatcher)
    {
        mLevelState = levelState;
        mPathfinder = pathfinder;
        mEventDispatcher = eventDispatcher;
    }

    public void Update()
    {
        foreach (var combatPair in AttackerToTarget)
        {
            var attacker = mLevelState.GetCharacterWithId(combatPair.Key);
            var target = mLevelState.GetCharacterWithId(combatPair.Value);

            if (attacker == null || target == null)
            {
                continue;
            }

            // Target is not in range
            if (!IsInRange(attacker, target))
            {
                if (attacker.mMovementState == MovementState.Idle && attacker.CurrentState is CharacterState.Attacking or CharacterState.PlayerControl or CharacterState.ArchEnemyControl)
                {
                    MoveAttackerToTarget(attacker, target.Position);
                }
                continue;
            }

            attacker.Timer += 1;
            // Character has reached his goal
            attacker.StopMovement();

            // Fixed Animation for BombMagician
            if (attacker is BombMagician)
            {
                if (attacker.Timer % attacker.Delay > attacker.Delay - 30 && attacker.mCurrentAnimation != Animations.AttackAnimation)
                {
                    attacker.ChangeAnimation(Animations.AttackAnimation);
                }
            }

            // Character has not yet reached his attack speed timing
            if (attacker.Timer % attacker.Delay != 0)
            {
                continue;
            }

            switch (attacker.CombatType)
            {
                case CombatType.Melee:
                    MeleeAttack(attacker, target);
                    break;
                case CombatType.AoE:
                    AoEMeleeAttack(attacker, target);
                    break;
                case CombatType.Ranged:
                    RangedAttack(attacker, target);
                    break;
                case CombatType.SelfDamageAoE:
                    SelfDamageAoeMeleeAttack(attacker, target);
                    break;
            }
        }
    }

    /// <summary>
    /// Attacker deals melee damage towards target
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    private void MeleeAttack(Character attacker, Character target)
    {
        DealsDamage(attacker, target);
    }

    /// <summary>
    /// Attacker deals aoe damage around the target's position
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    private void AoEMeleeAttack(Character attacker, GameObject target)
    {
        var charactersInRange = mLevelState.QuadTree.SearchCharacters(new Rect(
            new Vector2(
                target.Position.X - (attacker.Range / 2),
                target.Position.Y - (attacker.Range / 2)),
            new Vector2(attacker.Range,
                attacker.Range)));

        foreach (var character in charactersInRange)
        {
            if (attacker.IsFriendly != character.IsFriendly)
            {
                DealsDamage(attacker, character);
            }
        }
    }

    private void SelfDamageAoeMeleeAttack(Character attacker, GameObject target) //Noch Up to change wegen balancing
    {
        var charactersInRange = mLevelState.QuadTree.SearchCharacters(new Rect(
            new Vector2(
                target.Position.X - attacker.Range,
                target.Position.Y - attacker.Range),
            new Vector2(attacker.Range * 2,
                attacker.Range * 2)));

        foreach (var character in charactersInRange)
        {
            DealsDamage(attacker, character);
        }
    }

    private void RangedAttack(Character attacker, GameObject target)
    {
        ShootProjectile(attacker, target);
        attacker.Timer = 1;
    }

    private void ShootProjectile(Character attacker, GameObject target)
    {
        var direction = target.Position - attacker.Position;
        direction.Normalize();
        var start = attacker.Position;
        var destination = attacker.Position + attacker.Range * 2 * direction; //increased Range
        var projectile = CreateProjectile(attacker, start, destination);
        if (projectile != null)
        {
            mLevelState.AddProjectile(projectile);
            mEventDispatcher.SendAudioRequest(new SoundEvent(projectile.mSpawnSound, start));
        }
    }

    public void SetTarget(string attackerId, Character target)
    {
        RemoveTarget(attackerId);

        AttackerToTarget[attackerId] = target.Id;
        if (Attackers.ContainsKey(target.Id))
        {
            Attackers[target.Id].Add(attackerId);
            return;
        }

        var attackerIds = new HashSet<string>() { attackerId };
        Attackers[target.Id] = attackerIds;
        if (!target.IsFriendly)
        {
            target.Selected = true;
        }
    }

    public void SetTargetWithId(string attackerId, string targetId)
    {
        var target = mLevelState.GetCharacterWithId(targetId);
        if (target != null)
        {
            SetTarget(attackerId, target);
        }
    }

    public void RemoveTarget(string attackerId)
    {
        if (!AttackerToTarget.ContainsKey(attackerId))
        {
            return;
        }
        var currentTargetId = AttackerToTarget[attackerId];
        if (!Attackers.ContainsKey(currentTargetId))
        {
            return;
        }
        Attackers[currentTargetId].Remove(attackerId);
        if (Attackers[currentTargetId].Count == 0)
        {
            Attackers.Remove(currentTargetId);
            var enemy = mLevelState.GetEnemyWithId(currentTargetId);
            if (enemy != null)
            {
                enemy.Selected = false;
            }

        }
        AttackerToTarget.Remove(attackerId);
    }

    public void MoveAttackerToTarget(Character attacker, Vector2 targetPos)
    {
        var path = mPathfinder.CalculatePath(attacker.Position.ToGrid(), targetPos.ToGrid());
        attacker.SetPath(path);
    }

    private bool IsInRange(Character attacker, GameObject target)
    {
        return (attacker.Position - target.Position).Length() < attacker.Range + 32;
    }

    /// <summary>
    /// Deals damage and resets a character's attack timer
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    private void DealsDamage(Character attacker, Character target)
    {
        attacker.ChangeAnimation(Animations.AttackAnimation);
        attacker.Timer = 1;
        if (target is Summoner)
        {
            mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.SummonerHit, target.Position));
            if (this.mLevelState.InTechDemo)
            {
                return;
            }
        }
        target.GetHit((int)(attacker.Damage + attacker.DamageUp.Active * attacker.DamageUp.Strength), attacker.Element.ElementalEffectiveness(target.Element));
        mEventDispatcher.SendAudioRequest(target.IsFriendly
            ? new SoundEvent(SoundEffects.MeleeAlliedHit, target.Position)
            : new SoundEvent(SoundEffects.MeleeEnemyHit, target.Position));
    }

    private Projectile? CreateProjectile(Character attacker, Vector2 start, Vector2 destination)
    {
        attacker.ChangeAnimation(Animations.AttackAnimation);
        switch (attacker)
        {
            case Demon:
                return new SulfurChunk(start, destination, attacker.IsFriendly, attacker.Id, (int)(attacker.Damage + (attacker.DamageUp.Active * attacker.DamageUp.Strength)), attacker.Element);
            case WaterElemental:
                return new WaterDroplet(start, destination, attacker.IsFriendly, attacker.Id, (int)(attacker.Damage + (attacker.DamageUp.Active * attacker.DamageUp.Strength)), attacker.Element);
            case Pirate:
                return new GunShot(start, destination, attacker.IsFriendly, attacker.Id, (int)(attacker.Damage + (attacker.DamageUp.Active * attacker.DamageUp.Strength)), attacker.Element);
            case FrontPensioner:
                return new Stick(start, destination, attacker.IsFriendly, attacker.Id, (int)(attacker.Damage + (attacker.DamageUp.Active * attacker.DamageUp.Strength)), attacker.Element);
            default:
                return null;
        }
    }
}