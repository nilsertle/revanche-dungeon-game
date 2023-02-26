using Revanche.Core;
using Revanche.GameObjects;
using System.Security.Cryptography;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Revanche.Extensions;
using Revanche.GameObjects.HostileUnits;
using Revanche.GameObjects.Projectiles;

namespace Revanche.AI.HostileSummonBehaviour;

public sealed class ArchEnemyBehaviour : IEnemyBehaviour
{
    private CharacterState EnemyState => mEnemy.CurrentState;
    private Archenemy mEnemy;

    [JsonProperty] private bool mCanWalk;
    [JsonProperty] private bool mCanSummon;
    [JsonProperty] private bool mCanThrowTrap;

    [JsonProperty] private bool mCanFlee;

    [JsonProperty] private readonly List<string> mControlledSummons;

    [JsonProperty] private readonly Timer mWalkTimer;

    [JsonProperty] private readonly Timer mSummonTimer;

    [JsonProperty] private readonly Timer mBearTrapTimer;

    [JsonProperty] private int mSummonCapacity;

    [JsonProperty] private int mBearTrapAmount;

    private const int UnitThreshold = 5;
    private const int BearTrapThreshold = 10;
    private const int WalkTimer = 5000;
    private const int SummonTimer = 4000;
    private const int BearTrapTimer = 5000;
    private const int BearTrapDamage = 100;

    private const float FlightResponseHp = 0.2f;
    private const float FightOnEnemyHp = 0.5f;

    private const int ElementalPriority = 3;
    private const int CombatTypePriority = 2;
    private const int HealthPriority = 1;

    private const float HpEvaluationThreshold = 0.5f;


    public ArchEnemyBehaviour(Archenemy enemy)
    {
        mEnemy = enemy;
        mCanFlee = true;
        mSummonCapacity = Math.Min(enemy.Skills[ElementType.Fire], 3);
        mBearTrapAmount = BearTrapThreshold;
        mControlledSummons = new List<string>();

        // Walk timer setup
        mCanWalk = true;
        mWalkTimer = new Timer(WalkTimer);
        mWalkTimer.AutoReset = true;
        mWalkTimer.Elapsed += WalkTimerElapsed;
        mWalkTimer.Start();

        // Summon timer setup
        mCanSummon = true;
        mSummonTimer = new Timer(SummonTimer);
        mSummonTimer.AutoReset = true;
        mSummonTimer.Elapsed += SummonTimerElapsed;

        // Bear Trap timer setup
        mCanThrowTrap = true;
        mBearTrapTimer = new Timer(BearTrapTimer);
        mBearTrapTimer.AutoReset = true;
        mBearTrapTimer.Elapsed += BearTrapTimerElapsed;
    }

    public void UpdateState(LevelState levelState, GameLogic gameLogic)
    {
        // State independent behaviour
        UpdateSummonList(levelState);

        if (mControlledSummons.Count <= UnitThreshold && mEnemy.CurrentState != CharacterState.Fleeing)
        {
            RecruitHostile(levelState);
        }

        // State dependent behaviour
        switch (EnemyState)
        {
            case CharacterState.Idle:
                break;
            case CharacterState.Patrolling:
                Patrolling(levelState, gameLogic);
                break;
            case CharacterState.Attacking:
                Attacking(levelState, gameLogic);
                break;
            case CharacterState.Fleeing:
                Fleeing(levelState, gameLogic);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Fleeing(LevelState levelState, GameLogic gameLogic)
    {
        if (mCanThrowTrap && mBearTrapAmount > 0)
        {
            levelState.AddProjectile(new BearTrap(mEnemy.Position, new Vector2(0, 0), mEnemy.IsFriendly, mEnemy.Id, BearTrapDamage, ElementType.Neutral));
            mBearTrapAmount--;
            mCanThrowTrap = false;
        }

        if (mEnemy.mMovementState != MovementState.Idle)
        {
            return;
        }

        mBearTrapTimer.Stop();
        mEnemy.CurrentState = CharacterState.Patrolling;
        GatherControlledSummons(levelState, gameLogic);
    }

    private void Attacking(LevelState levelState, GameLogic gameLogic)
    {
        var strategy = EvaluateCombatState(levelState);
        switch (strategy)
        {
            case ArchEnemyStrategy.FightOn:
                MakeCombatDecisions(levelState, gameLogic);
                break;
            case ArchEnemyStrategy.AbandonFight:
                if (mCanFlee)
                {
                    AbandonControlledSummons(levelState, gameLogic);
                    mEnemy.CurrentState = CharacterState.Fleeing;
                    mCanFlee = false;
                    mSummonTimer.Stop();
                    mBearTrapTimer.Start();
                }
                else
                {
                    MakeCombatDecisions(levelState, gameLogic);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Patrolling(LevelState levelState, GameLogic gameLogic)
    {
        if (TargetInRange(levelState))
        {
            mEnemy.CurrentState = CharacterState.Attacking;
            mSummonTimer.Start();
        }

        if (mCanWalk)
        {
            SetRandomPath(levelState, gameLogic);
        }
    }

    private ArchEnemyStrategy EvaluateCombatState(LevelState levelState)
    {
        if (mEnemy.CurrentLifePoints < mEnemy.MaxLifePoints * FlightResponseHp)
        {
            return ArchEnemyStrategy.AbandonFight;
        }

        return mSummonCapacity switch
        {
            0 when mControlledSummons.Count == 0 => ArchEnemyStrategy.AbandonFight,
            > 0 when mEnemy.CurrentLifePoints >= FightOnEnemyHp * mEnemy.MaxLifePoints => ArchEnemyStrategy.FightOn,
            _ => !EvaluateChanceToWin(levelState) ? ArchEnemyStrategy.AbandonFight : ArchEnemyStrategy.FightOn
        };
    }

    private void MakeCombatDecisions(LevelState levelState, GameLogic gameLogic)
    {
        // The Archenemy and his summons act as a hive mind
        // Their entire vision is the union of all the visions
        // of the individual characters, so they can communicate
        // with each other what they see
        if (UnitThreshold - mControlledSummons.Count > 0 && mCanSummon && mSummonCapacity > 0)
        { 
            SummonHostile(levelState);
        }

        var controlledSummons = GetControlledSummons(levelState);
        var targetsInVision = GetGroupTargets(levelState);

        if (!MemberInCombat(levelState) && targetsInVision.Count == 0)
        {
            mEnemy.CurrentState = CharacterState.Patrolling;
            GatherControlledSummons(levelState, gameLogic);
            return;
        }

        // Decide upon the best target for every summon controlled by the Archenemy
        if (targetsInVision.Count == 0)
        {
            return;
        }

        gameLogic.AttackCharacter(mEnemy, GetPriorityTarget(mEnemy, targetsInVision));
        foreach (var controlledSummon in controlledSummons)
        {
            gameLogic.AttackCharacter(controlledSummon, GetPriorityTarget(controlledSummon, targetsInVision));
        }
    }

    // Finds the best target for a given hostile summon by comparing the targets against
    // each other, using a metric specified in the Priority function
    private Character GetPriorityTarget(Character controlledSummon, List<Character> targets)
    {
        return targets.MaxBy(target => Priority(controlledSummon, target));
    }

    private int Priority(Character controlledSummon, Character potentialTarget)
    {
        var priority = 0;
        if (controlledSummon.Element.ElementIsEffective(potentialTarget.Element))
        {
            priority += ElementalPriority;
        }

        switch (controlledSummon.CombatType)
        {
            case CombatType.Ranged when potentialTarget.CombatType == CombatType.Melee:
            case CombatType.Melee or CombatType.AoE or CombatType.SelfDamageAoE when potentialTarget.CombatType == CombatType.Ranged:
                priority += CombatTypePriority;
                break;
        }

        if (controlledSummon.CurrentLifePoints > potentialTarget.CurrentLifePoints)
        {
            priority += HealthPriority;
        }

        return priority;
    }

    private bool EvaluateChanceToWin(LevelState levelState)
    {
        var controlledSummons = GetControlledSummons(levelState);
        if (controlledSummons.Count == 0)
        {
            return false;
        }

        return controlledSummons.Sum(summon => (float)summon.CurrentLifePoints / summon.MaxLifePoints) >= HpEvaluationThreshold;
    }

    private void SetRandomPath(LevelState levelState, GameLogic gameLogic)
    {
        if (mEnemy.mMovementState == MovementState.Moving)
        {
            return;
        }

        var gridCoordinates = mEnemy.Position.ToGrid();
        var neighbors = levelState.GameMap.PassableNeighbors(gridCoordinates).ToList();
        var randomInt = RandomNumberGenerator.GetInt32(0, neighbors.Count);
        gameLogic.MoveCharacter(mEnemy, Camera.TileCenterToWorld(neighbors[randomInt]));
        GatherControlledSummons(levelState, gameLogic);
        mCanWalk = false;
    }

    private void GatherControlledSummons(LevelState levelState, GameLogic gameLogic)
    {
        var controlledSummons = GetControlledSummons(levelState);
        var gridCoordinates = mEnemy.Position.ToGrid();
        var neighbors = levelState.GameMap.PassableNeighbors(gridCoordinates).ToList();
        foreach (var controlledSummon in controlledSummons)
        {
            var randomInt = RandomNumberGenerator.GetInt32(0, neighbors.Count);
            gameLogic.MoveCharacter(controlledSummon, Camera.TileCenterToWorld(neighbors[randomInt]));
        }
    }

    private void AbandonControlledSummons(LevelState levelState, GameLogic gameLogic)
    {
        var fleeTarget = (from kvp in levelState.HostileSummons where !mControlledSummons.Contains(kvp.Key) select kvp.Value).FirstOrDefault();
        var controlledSummons = GetControlledSummons(levelState);
        foreach (var controlledSummon in controlledSummons)
        {
            controlledSummon.CurrentState = CharacterState.Attacking;
        }
        mControlledSummons.Clear();
        if (fleeTarget != null)
        {
            gameLogic.MoveCharacter(mEnemy, fleeTarget.Position);
        }
    }

    private void RecruitHostile(LevelState levelState)
    {
        var hostiles = levelState.QuadTree.SearchCharacters(mEnemy.VisionRectangle).Where(c => !c.IsFriendly).ToList();
        foreach (var hostileSummon in hostiles)
        {
            if (mControlledSummons.Contains(hostileSummon.Id) || hostileSummon.Id == mEnemy.Id)
            {
                continue;
            }

            mControlledSummons.Add(hostileSummon.Id);
            hostileSummon.CurrentState = CharacterState.ArchEnemyControl;

            if (mControlledSummons.Count >= UnitThreshold)
            {
                break;
            }
        }
    }

    private void SummonHostile(LevelState levelState)
    {
        var element = GetElement(levelState);
        var hostile = CreateHostile(element);
        levelState.AddHostileSummon(hostile);
        mCanSummon = false;
        mSummonCapacity--;
    }

    private Summon CreateHostile(ElementType element)
    {
        Summon hostileSummon;
        switch (element)
        {
            case ElementType.Fire:
                hostileSummon = new ConanTheBarbarian(Camera.TileCenterToWorld(mEnemy.Position.ToGrid()), mEnemy.Skills[element]);
                hostileSummon.CurrentState = CharacterState.ArchEnemyControl;
                return hostileSummon;
            case ElementType.Water:
                hostileSummon = new Pirate(Camera.TileCenterToWorld(mEnemy.Position.ToGrid()), mEnemy.Skills[element]);
                hostileSummon.CurrentState = CharacterState.ArchEnemyControl;
                return hostileSummon;
            case ElementType.Lightning:
                hostileSummon = new Paladin(Camera.TileCenterToWorld(mEnemy.Position.ToGrid()), mEnemy.Skills[element]);
                hostileSummon.CurrentState = CharacterState.ArchEnemyControl;
                return hostileSummon;
            case ElementType.Magic:
                hostileSummon = new BombMagician(Camera.TileCenterToWorld(mEnemy.Position.ToGrid()), mEnemy.Skills[element]);
                hostileSummon.CurrentState = CharacterState.ArchEnemyControl;
                return hostileSummon;
            case ElementType.Ghost:
                hostileSummon = new FrontPensioner(Camera.TileCenterToWorld(mEnemy.Position.ToGrid()), mEnemy.Skills[element]);
                hostileSummon.CurrentState = CharacterState.ArchEnemyControl;
                return hostileSummon;
            default:
                throw new ArgumentOutOfRangeException(nameof(element), element, null);
        }
    }

    private ElementType GetElement(LevelState levelState)
    {
        var controlledSummons = GetControlledSummons(levelState);
        var elementCount = new Dictionary<ElementType, int>()
        {
            { ElementType.Fire , 0},
            { ElementType.Water , 0},
            { ElementType.Lightning , 0},
            { ElementType.Ghost , 0},
            { ElementType.Magic , 0},
        };
        foreach (var controlledSummon in controlledSummons)
        {
            elementCount[controlledSummon.Element]++;
        }

        return elementCount.MinBy(kvp => kvp.Value).Key;
    }

    private bool TargetInRange(LevelState levelState)
    {
        var charactersInRange = GetGroupTargets(levelState);
        return charactersInRange.Count != 0 
               || GetControlledSummons(levelState).Exists(summon => levelState.AttackerToTarget.ContainsKey(summon.Id)) 
               || levelState.AttackerToTarget.ContainsKey(mEnemy.Id);
    }

    private void UpdateSummonList(LevelState levelState)
    {
        // Remove dead units
        mControlledSummons.RemoveAll(hostile => !levelState.HostileSummons.ContainsKey(hostile));
    }

    private List<Character> GetGroupTargets(LevelState levelState)
    {
        var targets = levelState.QuadTree.SearchCharacters(mEnemy.VisionRectangle)
            .Where(character => character.IsFriendly).ToList();
        foreach (var controlledSummon in GetControlledSummons(levelState))
        {
            targets.AddRange(levelState.QuadTree.SearchCharacters(controlledSummon.VisionRectangle)
                    .Where(character => character.IsFriendly && !targets.Contains(character)));
        }
        return targets;
    }

    private List<Character> GetControlledSummons(LevelState levelState)
    {
        var charList = new List<Character>();
        foreach (var controlledSummonId in mControlledSummons)
        {
            var controlledSummon = levelState.GetSummonWithId(controlledSummonId);
            if (controlledSummon != null)
            {
                charList.Add(controlledSummon);
            }
        }
        return charList;
    }

    private bool MemberInCombat(LevelState levelState)
    {
        return levelState.AttackerToTarget.ContainsKey(mEnemy.Id) ||
               GetControlledSummons(levelState).Any(summon => levelState.AttackerToTarget.ContainsKey(summon.Id));
    }

    public void Initialize(Character enemy)
    {
        mEnemy = (Archenemy)enemy;
    }

    private void WalkTimerElapsed(object sender, ElapsedEventArgs e)
    {
        mCanWalk = true;
    }

    private void SummonTimerElapsed(object sender, ElapsedEventArgs e)
    {
        mCanSummon = true;
    }

    private void BearTrapTimerElapsed(object sender, ElapsedEventArgs e)
    {
        mCanThrowTrap = true;
    }
}

public enum ArchEnemyStrategy
{
    FightOn, AbandonFight
}