#nullable enable
using System;
using System.Linq;
using System.Timers;
using Revanche.Core;
using Revanche.GameObjects;
using Newtonsoft.Json;
using Revanche.Extensions;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Revanche.GameObjects.HostileUnits;

namespace Revanche.AI.HostileSummonBehaviour;

public class PaladinBehaviour : EnemyBehaviour, IEnemyBehaviour
{
    private const int TimerInterval = 5000;

    private CharacterState EnemyState => mEnemy.CurrentState;

    [JsonProperty] private Timer mTimer;
    [JsonProperty] private bool mWalk;
    
    public PaladinBehaviour(Character paladin)
    {
        mEnemy = paladin;
        mTimer = new Timer(TimerInterval);
        mTimer.Elapsed += TimerElapsed;
        mTimer.AutoReset = true;
        mTimer.Start();
        mWalk = false;
    }
    
    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        mWalk = true;
    }

    // This function now takes the game Logic. All functionality like moving a enemy,
    // attacking a enemy and so on should only be done via the gameLogic to ensure
    // correctness
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
                Fleeing();
                break;
            case CharacterState.ArchEnemyControl:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void Fleeing()
    {
        // The Paladin does not flee
    }

    private void Attacking(LevelState levelState)
    {
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
            Taunt(levelState, gameLogic);
            return;
        }

        // Walks around randomly until an enemy is in sight
        var target = FindTargetInVision(levelState);
        if (target != null)
        {
            mEnemy.CurrentState = CharacterState.Attacking;
            gameLogic.AttackCharacter(mEnemy, target);
            Taunt(levelState, gameLogic);
            return;
        }
        
        // Walks to a friend
        if (SetPathToFriend(levelState, gameLogic))
        {
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

    private bool SetPathToFriend(LevelState levelState, GameLogic gameLogic)
    {
        if (mEnemy.mMovementState == MovementState.Moving)
        {
            return true;
        }

        if (mWalk)
        {
            return SetPathToFriendHelper(levelState, gameLogic);
        }

        return false;
    }

    private bool SetPathToFriendHelper(LevelState levelState, GameLogic gameLogic)
    {
        if (FriendIsAttacked(levelState, gameLogic))
        {
            return true;
        }
        var friendsInRange = levelState.QuadTree.SearchCharacters(mEnemy.VisionRectangle).Where(c => !c.IsFriendly)
            .ToList();
        if (friendsInRange.Count != 0)
        {
            foreach (var friend in friendsInRange)
            {
                if (friend is Paladin)
                {
                    continue;
                }
                gameLogic.MoveCharacter(mEnemy, friend.Position);
                mWalk = false;
                return true;
            }
        }
        return false;
    }
    
    private void Taunt(LevelState levelState, GameLogic gameLogic)
    {
        var charactersInRange = levelState.QuadTree.SearchCharacters(mEnemy.VisionRectangle).Where(c => c.IsFriendly).ToList();
        if (charactersInRange.Count > 0)
        {
            gameLogic.AttackCharacter(charactersInRange[0], mEnemy);
        }
    }

    private bool FriendIsAttacked(LevelState levelState, GameLogic gameLogic)
    {
        var friendsInRange = levelState.QuadTree.SearchCharacters(new Rect(
            new Vector2(
                // Paladin has higher Vision for friends, that are attacked
                // Can still be nerfed
                mEnemy.Position.X - (2 * mEnemy.Vision),
                mEnemy.Position.Y - (2 * mEnemy.Vision)),
            new Vector2(4 * mEnemy.Vision,
                4 * mEnemy.Vision))).Where(c => !c.IsFriendly).ToList();
        foreach (var friend in friendsInRange)
        {
            // Paladin helps the friend
            if (friend.CurrentState == CharacterState.Attacking 
                || friend.CurrentState == CharacterState.Fleeing)
            {
                gameLogic.MoveCharacter(mEnemy, friend.Position);
                mWalk = false;
                return true;
            }
        }
        return false; 
    }
}