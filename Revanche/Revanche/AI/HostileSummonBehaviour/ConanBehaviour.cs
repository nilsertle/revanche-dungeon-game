#nullable enable
using System;
using System.Linq;
using System.Timers;
using Revanche.Core;
using Revanche.GameObjects;
using Newtonsoft.Json;
using Revanche.Extensions;
using System.Security.Cryptography;
using Revanche.Sound;

namespace Revanche.AI.HostileSummonBehaviour;

public sealed class ConanBehaviour : EnemyBehaviour, IEnemyBehaviour
{
    private const int TimerInterval = 3000;
    private const int BuffDuration = 8;
    private const int BuffStrength = 5;

    private CharacterState EnemyState => mEnemy.CurrentState;

    [JsonProperty] private readonly Timer mTimer;
    [JsonProperty] private bool mWalk;

    public ConanBehaviour(Character enemy)
    {
        mEnemy = enemy;
        mTimer = new Timer(TimerInterval);
        mTimer.Elapsed += TimerElapsed;
        mTimer.AutoReset = true;
        mTimer.Start();
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
                Attacking(levelState);
                break;
            case CharacterState.Fleeing:
                // Conan doesn't flee
                break;
            case CharacterState.ArchEnemyControl:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void Attacking(LevelState levelState)
    {
        // Check if target is still alive
        if (!levelState.AttackerToTarget.ContainsKey(mEnemy.Id))
        {
            mEnemy.CurrentState = CharacterState.Patrolling;
        }
    }

    private void Patrolling(LevelState levelState, GameLogic gameLogic)
    {

        if (levelState.AttackerToTarget.ContainsKey(mEnemy.Id))
        {
            mEnemy.CurrentState = CharacterState.Attacking;
            TriggerWarScream(levelState);
            return;
        }

        var target = FindTargetInVision(levelState);
        if (target != null)
        {
            TriggerWarScream(levelState);
            gameLogic.AttackCharacter(mEnemy, target);
            mEnemy.CurrentState = CharacterState.Attacking;
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
        var gridCoordinates = mEnemy.Position.ToGrid();
        var neighbors = levelState.GameMap.PassableNeighbors(gridCoordinates).ToList();
        var randomInt = RandomNumberGenerator.GetInt32(0, neighbors.Count);
        var goals = levelState.GameMap.PassableNeighbors(neighbors[randomInt]).ToList();
        var randomInt2 = RandomNumberGenerator.GetInt32(0, goals.Count);
        gameLogic.MoveCharacter(mEnemy, Camera.TileCenterToWorld(goals[randomInt2]));
        mWalk = false;
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        mWalk = true;
    }

    private void TriggerWarScream(LevelState levelState)
    {
        levelState.EventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ConanSpecial, mEnemy.Position));
        var charactersInRange = levelState.QuadTree.SearchCharacters(mEnemy.VisionRectangle).Where(c => !c.IsFriendly).ToList();
        foreach (var enemy in charactersInRange)
        {
            enemy.DamageUp = TemporaryEffect.CreateTemporaryEffect(BuffDuration, BuffStrength);
        }
    }
}