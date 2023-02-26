#nullable enable
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Revanche.AchievementSystem;
using Revanche.AI;
using Revanche.Extensions;
using Revanche.GameObjects;
using Revanche.GameObjects.Environment;
using Revanche.GameObjects.FriendlyUnits;
using Revanche.GameObjects.Projectiles;
using Revanche.Managers;
using Revanche.Map.Pathfinding;
using Revanche.Sound;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Revanche.Core;

public sealed class GameLogic
{
    private const int LowSummonCost = 1;

    private const int ProjectileOffset = 20;
    private const int FireBallCost = 20;
    private const int SpeedSpellCost = 50;

    private readonly LevelState mLevelState;
    private readonly EventDispatcher mEventDispatcher;
    private readonly EnemyBehaviourHandler mAiHandler;
    private readonly CollisionHandler mCollisionHandler;
    private readonly IPathfinder mPathfinder;
    private readonly CombatHandler mCombatHandler;

    public GameLogic(LevelState levelState, EventDispatcher eventDispatcher)
    {
        mLevelState = levelState;
        mEventDispatcher = eventDispatcher;
        mAiHandler = new EnemyBehaviourHandler(levelState);
        mPathfinder = new AStarPathfinder(levelState);
        mCombatHandler = new CombatHandler(levelState, mPathfinder, eventDispatcher);
        mCollisionHandler = new CollisionHandler(levelState, eventDispatcher, mCombatHandler);
    }

    internal void Update(float deltaTime)
    {
        mAiHandler.Update(mLevelState, this);
        mCollisionHandler.HandleCollisions(deltaTime);
        mCombatHandler.Update();
    }

    internal void ConsumeItem(MainCharacter mainCharacter)
    {
        var items = mLevelState.QuadTree.SearchItems(mainCharacter.Hitbox);
        if (items.Any())
        {
            mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.UsePotion, items[0].Position));
            mLevelState.ConsumeItem(items[0]);
        }
    }

    internal void SelectCharacter(Character character)
    {
        character.Selected = !character.Selected;
    }

    internal void MoveCharacter(Character character, Vector2 destination)
    {
        mCombatHandler.RemoveTarget(character.Id);
        var path = mPathfinder.CalculatePath(character.Position.ToGrid(), destination.ToGrid());
        character.SetPath(path);
    }

    internal void MoveCharacters(List<Character> characters, Vector2 destination)
    {
        foreach (var character in characters)
        {
            mCombatHandler.RemoveTarget(character.Id);
        }
        PathExtensions.MultiP(mLevelState, mPathfinder, characters, destination);
    }

    internal void SummonFriendlyMonster(SummonType? type, Vector2 position)
    {
        if (!mLevelState.Summoner.CanSummon() || !mLevelState.Summoner.CanSpawnCharacterInRange(position, (int)(mLevelState.Summoner.SummonRange * Game1.sScaledPixelSize)) ||
            !mLevelState.IsSpaceFree(position) || !mLevelState.OnGround(position))
        {
            mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.InvalidAction, null));
            return;
        }

        position = Camera.TileCenterToWorld(position.ToGrid());
        EnvironmentalItem? leftOfSummoner = null;
        EnvironmentalItem? topOfSummoner = null;
        EnvironmentalItem? rightOfSummoner = null;
        EnvironmentalItem? bottomOfSummoner = null;
        var posLeft = mLevelState.Summoner.Position + new Vector2(-1, 0) * Game1.sScaledPixelSize;
        var posTop = mLevelState.Summoner.Position + new Vector2(0, -1) * Game1.sScaledPixelSize;
        var posRight = mLevelState.Summoner.Position + new Vector2(1,0) * Game1.sScaledPixelSize;
        var posBottom = mLevelState.Summoner.Position + new Vector2(0,1) * Game1.sScaledPixelSize;
        switch (type)
        {
            case SummonType.Demon:
                mLevelState.AddFriendlySummon(new Demon(position, mLevelState.Summoner.Skills[ElementType.Fire]));
                mLevelState.Summoner.Souls -= LowSummonCost;
                leftOfSummoner = new EnvironmentalItem(posLeft,
                    EnvironmentalAnimations.FireIcon,
                    EnvironmentalMode.SingleAnimation);
                topOfSummoner = new EnvironmentalItem(posTop,
                    EnvironmentalAnimations.FireIcon,
                    EnvironmentalMode.SingleAnimation); 
                rightOfSummoner = new EnvironmentalItem(posRight,
                    EnvironmentalAnimations.FireIcon,
                    EnvironmentalMode.SingleAnimation); 
                bottomOfSummoner = new EnvironmentalItem(posBottom,
                    EnvironmentalAnimations.FireIcon,
                    EnvironmentalMode.SingleAnimation);
                break;
            case SummonType.Skeleton:
                mLevelState.AddFriendlySummon(new Skeleton(position, mLevelState.Summoner.Skills[ElementType.Ghost]));
                mLevelState.Summoner.Souls -= LowSummonCost;
                leftOfSummoner = new EnvironmentalItem(posLeft,
                    EnvironmentalAnimations.GhostIcon,
                    EnvironmentalMode.SingleAnimation);
                topOfSummoner = new EnvironmentalItem(posTop,
                    EnvironmentalAnimations.GhostIcon,
                    EnvironmentalMode.SingleAnimation);
                rightOfSummoner = new EnvironmentalItem(posRight,
                    EnvironmentalAnimations.GhostIcon,
                    EnvironmentalMode.SingleAnimation);
                bottomOfSummoner = new EnvironmentalItem(posBottom,
                    EnvironmentalAnimations.GhostIcon,
                    EnvironmentalMode.SingleAnimation);
                break;
            case SummonType.StormCloud:
                mLevelState.AddFriendlySummon(new StormCloud(position, mLevelState.Summoner.Skills[ElementType.Lightning]));
                mLevelState.Summoner.Souls -= LowSummonCost;
                leftOfSummoner = new EnvironmentalItem(posLeft,
                    EnvironmentalAnimations.LightningIcon,
                    EnvironmentalMode.SingleAnimation);
                topOfSummoner = new EnvironmentalItem(posTop,
                    EnvironmentalAnimations.LightningIcon,
                    EnvironmentalMode.SingleAnimation);
                rightOfSummoner = new EnvironmentalItem(posRight,
                    EnvironmentalAnimations.LightningIcon,
                    EnvironmentalMode.SingleAnimation);
                bottomOfSummoner = new EnvironmentalItem(posBottom,
                    EnvironmentalAnimations.LightningIcon,
                    EnvironmentalMode.SingleAnimation);
                break;
            case SummonType.WaterElemental:
                mLevelState.AddFriendlySummon(new WaterElemental(position, mLevelState.Summoner.Skills[ElementType.Water]));
                mLevelState.Summoner.Souls -= LowSummonCost;
                leftOfSummoner = new EnvironmentalItem(posLeft,
                    EnvironmentalAnimations.WaterIcon,
                    EnvironmentalMode.SingleAnimation);
                topOfSummoner = new EnvironmentalItem(posTop,
                    EnvironmentalAnimations.WaterIcon,
                    EnvironmentalMode.SingleAnimation);
                rightOfSummoner = new EnvironmentalItem(posRight,
                    EnvironmentalAnimations.WaterIcon,
                    EnvironmentalMode.SingleAnimation);
                bottomOfSummoner = new EnvironmentalItem(posBottom,
                    EnvironmentalAnimations.WaterIcon,
                    EnvironmentalMode.SingleAnimation);
                break;
            case SummonType.MagicSeedling:
                mLevelState.AddFriendlySummon(new MagicSeedling(position, mLevelState.Summoner.Skills[ElementType.Magic]));
                mLevelState.Summoner.Souls -= LowSummonCost;
                leftOfSummoner = new EnvironmentalItem(posLeft,
                    EnvironmentalAnimations.MagicIcon,
                    EnvironmentalMode.SingleAnimation);
                topOfSummoner = new EnvironmentalItem(posTop,
                    EnvironmentalAnimations.MagicIcon,
                    EnvironmentalMode.SingleAnimation);
                rightOfSummoner = new EnvironmentalItem(posRight,
                    EnvironmentalAnimations.MagicIcon,
                    EnvironmentalMode.SingleAnimation);
                bottomOfSummoner = new EnvironmentalItem(posBottom,
                    EnvironmentalAnimations.MagicIcon,
                    EnvironmentalMode.SingleAnimation);
                break;
        }
        mLevelState.AddToMutableUseAble(new List<GameObject>{ leftOfSummoner!, topOfSummoner!, rightOfSummoner!, bottomOfSummoner! });
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.GenericSummoning, position));
    }
     
    internal void ShootFireball(Vector2 mousePosition)
    {
        if (mLevelState.Summoner.CurrentMana < FireBallCost || mLevelState.Summoner.FireBallCoolDown < Summoner.CoolDownLimitFire)
        {
            return;
        }

        mLevelState.Summoner.FireBallCoolDown = 0;
        mLevelState.Summoner.ActualMana -= FireBallCost;
        if ((int)mLevelState.Summoner.ActualMana == 0)
        {
            mEventDispatcher.SendAchievementEvent(AchievementType.MeinPersönlicherTiefpunkt);
        }

        var direction = mousePosition - mLevelState.Summoner.Position;
        direction.Normalize();
        var start = mLevelState.Summoner.Position + ProjectileOffset * direction; //might look stupid but feels better in small areas
        var destination = mLevelState.Summoner.Position + (mLevelState.Summoner.Range + 4 * Game1.sScaledPixelSize) * direction;
        var fireball = new Fireball(start, destination, true, mLevelState.Summoner.Id, mLevelState.Summoner.FireballDamage, ElementType.Neutral);
        mLevelState.AddProjectile(fireball);
        mEventDispatcher.SendAudioRequest(new SoundEvent(fireball.mSpawnSound, start));
    }
    
    internal void ShootHealSpell(Vector2 mousePosition)
    {
        var healSpellCost = (int)(mLevelState.Summoner.MaxLifePoints * mLevelState.Summoner.HealHpCost);
        if (mLevelState.Summoner.CurrentLifePoints <= healSpellCost || mLevelState.Summoner.HealingSpellCoolDown < Summoner.CoolDownLimitHeal)
        {
            return;
        }
        
        mLevelState.Summoner.HealingSpellCoolDown = 0;
        mLevelState.Summoner.CurrentLifePoints -= healSpellCost;
        var direction = mousePosition - mLevelState.Summoner.Position;
        direction.Normalize();
        var start = mLevelState.Summoner.Position + ProjectileOffset * direction;
        var destination = mLevelState.Summoner.Position + (mLevelState.Summoner.Range + 4 * Game1.sScaledPixelSize) * direction;
        // isFriendly is false bc it should hit ur Allies
        var healSpell = new HealSpell(start, destination, false, mLevelState.Summoner.Id, 0, ElementType.Neutral);
        mLevelState.AddProjectile(healSpell);
        mEventDispatcher.SendAudioRequest(new SoundEvent(healSpell.mSpawnSound, start));
        mLevelState.Summoner.SetColor(Color.Red);
    }
    
    internal void ShootSpeedSpell(Vector2 mousePosition)
    {
        if (mLevelState.Summoner.CurrentMana < SpeedSpellCost || mLevelState.Summoner.SpeedSpellCoolDown < Summoner.CoolDownLimitSpeed)
        {
            return;
        }

        mLevelState.Summoner.SpeedSpellCoolDown = 0;
        mLevelState.Summoner.ActualMana -= SpeedSpellCost;
        if ((int)mLevelState.Summoner.ActualMana == 0)
        {
            mEventDispatcher.SendAchievementEvent(AchievementType.MeinPersönlicherTiefpunkt);
        }
        mLevelState.Summoner.UseProjectileEffect(ProjectileEffect.Speed, mLevelState.Summoner.HealingStrength, mLevelState.Summoner.SpeedStrength);
        var direction = mousePosition - mLevelState.Summoner.Position;
        direction.Normalize();
        var start = mLevelState.Summoner.Position + ProjectileOffset * direction;
        var destination = mLevelState.Summoner.Position + (mLevelState.Summoner.Range + 2 * Game1.sScaledPixelSize) * direction;
        // isFriendly is false bc it should hit ur Allies
        var speedSpell = new SpeedSpell(start, destination, false, mLevelState.Summoner.Id, 0, ElementType.Neutral);
        mLevelState.AddProjectile(speedSpell);
        mEventDispatcher.SendAudioRequest(new SoundEvent(speedSpell.mSpawnSound, start));
    }
    

    internal void AttackCharacter(Character attacker, Character target)
    {
        mCombatHandler.SetTarget(attacker.Id, target);
        mCombatHandler.MoveAttackerToTarget(attacker, target.Position);
    }

    internal Character? FindTarget(Vector2 position)
    {
        var targets = mLevelState.QuadTree.PointSearchCharacters(position);
        return targets.FirstOrDefault();
    }

    
}