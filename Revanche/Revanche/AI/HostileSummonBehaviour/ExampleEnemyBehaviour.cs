using System;
using System.Linq;
using Revanche.Core;
using Revanche.GameObjects;
using System.Security.Cryptography;
using Revanche.Extensions;

namespace Revanche.AI.HostileSummonBehaviour;

/// <summary>
/// Example behaviour to show how AI could possibly work. I am not sure
/// if this is the best approach but it is the best I could think of
/// in a very short period of time. Let me know if you have better
/// ideas or issues with this system.
/// </summary>
public class ExampleEnemyBehaviour : EnemyBehaviour, IEnemyBehaviour
{
    private const float F09 = 0.9f;

    private CharacterState EnemyState => mEnemy.CurrentState;

    // mEnemy comes from EnemyBehaviour, not IEnemyBehaviour
    // It's a long story to explain why this is necesarry
    // For now, inherit from both and set the constructor like this
    // It allows us to save the behaviour and get easier access to the
    // enemy we want to modify. I might find a better solution in the future
    public ExampleEnemyBehaviour(Character enemy)
    {
        mEnemy = enemy;
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
                Attacking(levelState, gameLogic);
                break;
            case CharacterState.Fleeing:
                Fleeing();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
        if (mEnemy.CurrentLifePoints >= mEnemy.MaxLifePoints * F09)
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
        // Walks around randomly until an enemy is in sight
        if (IsTargetInRange(levelState, gameLogic))
        {
            mEnemy.CurrentState = CharacterState.Attacking;
            mEnemy.StopMovement();
            return;
        }
        SetRandomPath(levelState, gameLogic);
    }

    private bool IsTargetInRange(LevelState levelState, GameLogic gameLogic)
    {
        var charactersInRange = levelState.QuadTree.SearchCharacters(mEnemy.VisionRectangle).Where(c => c.IsFriendly).ToList();

        if (!charactersInRange.Any())
        {
            return false;
        }

        gameLogic.AttackCharacter(mEnemy, charactersInRange.First());

        return true;
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
    }
}