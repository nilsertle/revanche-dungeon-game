#nullable enable
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Timers;
using Newtonsoft.Json;
using Revanche.Core;
using Revanche.Extensions;
using Revanche.GameObjects;

namespace Revanche.AI.HostileSummonBehaviour;

internal sealed class FrontPensionerBehaviour: EnemyBehaviour, IEnemyBehaviour
{
    private const int TimerInterval = 8000;
    private const float FleeHpThreshold = 0.5f;

    private CharacterState EnemyState => mEnemy.CurrentState;
    [JsonProperty] private readonly Timer mTimer;
    [JsonProperty] private bool mWalk;

    public FrontPensionerBehaviour(Character frontPensioner)
    {
        mEnemy = frontPensioner;
        mTimer = new Timer(TimerInterval);
        mTimer.Elapsed += TimerElapsed;
        mTimer.AutoReset = true;
        mTimer.Start();
        mWalk = false;
    }
    
    private void Fleeing()
    {
        if (mEnemy.mMovementState == MovementState.Idle)
        {
            mEnemy.CurrentState = CharacterState.Idle;
        }
    }

    private void Attacking(LevelState levelState, GameLogic gameLogic)
    {
        if (!levelState.AttackerToTarget.ContainsKey(mEnemy.Id))
        {
            mEnemy.CurrentState = CharacterState.Patrolling;
            mTimer.Stop();
        }

        if (mEnemy.CurrentLifePoints >= mEnemy.MaxLifePoints * FleeHpThreshold)
        {
            return;
        }
        // State transition to fleeing after hp is low enough
        gameLogic.MoveCharacter(mEnemy,
            levelState.ArchEnemy?.Position ?? levelState.Ladder!.Position);
        
        mEnemy.CurrentState = CharacterState.Fleeing;
    }

    private void Patrolling(LevelState levelState, GameLogic gameLogic)
    {

        if (levelState.AttackerToTarget.ContainsKey(mEnemy.Id))
        {
            mEnemy.CurrentState = CharacterState.Attacking;
            mTimer.Stop();
            return;
        }

        // Walks around randomly until an enemy is in sight
        var target = FindTargetInVision(levelState);
        if (target != null)
        {
            mEnemy.CurrentState = CharacterState.Attacking;
            gameLogic.AttackCharacter(mEnemy, target);
            mTimer.Stop();
            return;
        }

        if (mWalk)
        {
            SetRandomPath(levelState, gameLogic);
        }
    }

    private Character? FindTargetInVision(LevelState levelState)
    {
        return levelState.QuadTree.SearchCharacters(mEnemy.VisionRectangle).FirstOrDefault(c => c.IsFriendly);
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
        // Don't forget that neighbors[randomInt] is already grid coordinates, but move enemy requires
        // the world position (obtainable via TileCenterToWorld)
        gameLogic.MoveCharacter(mEnemy, Camera.TileCenterToWorld(neighbors[randomInt]));
        mWalk = false;
    }
    public void UpdateState(LevelState levelState, GameLogic gameLogic)
    {
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
                Fleeing();
                break;
            case CharacterState.ArchEnemyControl:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        mWalk = true;
    }
}