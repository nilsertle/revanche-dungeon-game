#nullable enable
using Microsoft.Xna.Framework;
using Revanche.Core;
using Revanche.Extensions;
using Revanche.GameObjects;
using System;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Revanche.AI.HostileSummonBehaviour;

public sealed class PirateBehaviour : EnemyBehaviour, IEnemyBehaviour
{
    private const int I32 = 32;
    private const int I64 = 64;
    private const float FleeHpThreshold = 0.2f;
    private const float KitingHpThreshold = 0.7f;

    [JsonProperty] private string? mTarget;
    private CharacterState EnemyState => mEnemy.CurrentState;


    public PirateBehaviour(Character enemy)
    {
        mEnemy = enemy;
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
                Fleeing(levelState, gameLogic);
                break;
            case CharacterState.Kiting:
                Kiting(levelState, gameLogic);
                break;
            case CharacterState.ArchEnemyControl:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }



    // in this case Attacking includes kiting
    private void Attacking(LevelState levelState)
    {
        if (mTarget == null || levelState.GetCharacterWithId(mTarget) == null)
        {
            mTarget = null;
            mEnemy.CurrentState = CharacterState.Patrolling;
            return;
        }

        // if target can reach pirate start kiting
        //if (IsInRange(levelState.GetCharacterWithId(mTarget)!, mEnemy))
        if (Vector2.Distance(levelState.GetCharacterWithId(mTarget!)!.Position, mEnemy.Position) < Game1.sScaledPixelSize*2)
        {
            // keep distance
            mEnemy.CurrentState = CharacterState.Kiting;
        }

        // if level 3 or so also start hiding behind teammate (only makes sense if friendly fire is off for enemies)

        if (mEnemy.CurrentLifePoints < mEnemy.MaxLifePoints * FleeHpThreshold)
        {
            mEnemy.CurrentState = CharacterState.Fleeing;
        }
        // State transition to fleeing after hp is low enough (exchange this fleeing by actual fleeing and not kiting)
    }

    private void Patrolling(LevelState levelState, GameLogic gameLogic)
    {
        if (levelState.AttackerToTarget.ContainsKey(mEnemy.Id))
        {
            mEnemy.CurrentState = CharacterState.Attacking;
            mTarget = levelState.AttackerToTarget[mEnemy.Id];
            return;
        }

        // Walks around randomly until an enemy is in sight
        var target = FindTargetInVision(levelState);
        if (target != null)
        {
            mEnemy.CurrentState = CharacterState.Attacking;
            gameLogic.AttackCharacter(mEnemy, target);
            mTarget = target.Id;
            //mEnemy?.SetPath(new List<Vector2>());
            return;
        }
        SetRandomPath(levelState, gameLogic);
    }

    private bool IsInRange(Character attacker, GameObject target)
    {
        return (attacker.Position - target.Position).Length() < attacker.Range + I32 + (EnemyState == CharacterState.Kiting ? I64 : 0);
    }

    private Character? FindTargetInVision(LevelState levelState)
    {
        return levelState.QuadTree.SearchCharacters(mEnemy.VisionRectangle).FirstOrDefault(c => c.IsFriendly);
    }
      
    private void Fleeing(LevelState levelState, GameLogic gameLogic)
    {
        // move away but if in vision and out of the other range change to attack
        if (mEnemy.CurrentLifePoints < mEnemy.MaxLifePoints * KitingHpThreshold)
        {
            // hide and then attack
            SetRandomPath(levelState, gameLogic, PathBias.BehindNextHealthiestUnit);
            if (mEnemy.mMovementState != MovementState.Moving)
            {
                // keep distance
                gameLogic.AttackCharacter(mEnemy, levelState.GetCharacterWithId(mTarget!)!);
                mEnemy.CurrentState = CharacterState.Attacking;
            }
            return;
        }

        if (!IsInRange(levelState.GetCharacterWithId(mTarget!)!, mEnemy))
        {
            // keep distance
            gameLogic.AttackCharacter(mEnemy, levelState.GetCharacterWithId(mTarget!)!);
            mEnemy.CurrentState = CharacterState.Attacking;
        }
        else
        {
            // Move furthest away
            SetRandomPath(levelState, gameLogic, PathBias.FurthestAway);
        }
    }

    private void Kiting(LevelState levelState, GameLogic gameLogic)
    {
        if (mEnemy.CurrentLifePoints < mEnemy.MaxLifePoints * FleeHpThreshold)
        {
            mEnemy.CurrentState = CharacterState.Fleeing;
            return;
        }

        // move away but if in vision and out of the other range change to attack
        if (mTarget == null || levelState.GetCharacterWithId(mTarget) == null)
        {
            mTarget = null;
            mEnemy.CurrentState = CharacterState.Patrolling;
            return;
        }

        if (!IsInRange(levelState.GetCharacterWithId(mTarget!)!, mEnemy))
        {
            // keep distance
            gameLogic.AttackCharacter(mEnemy, levelState.GetCharacterWithId(mTarget!)!);
            mEnemy.CurrentState = CharacterState.Attacking;
            return;
        }

        // Move furthest away
        SetRandomPath(levelState, gameLogic, PathBias.FurthestAway);
    }

    private enum PathBias
    {
        FurthestAway, BehindNextHealthiestUnit, None
    }

    private void SetRandomPath(LevelState levelState, GameLogic gameLogic, PathBias bias = PathBias.None)
    {
        if (mEnemy.mMovementState == MovementState.Moving)
        {
            return;
        }

        var gridCoordinates = mEnemy.Position.ToGrid();
        var neighbors = levelState.GameMap.PassableNeighbors(gridCoordinates).ToList();
        var goal = new Vector2();
        switch (bias)
        {
            case PathBias.None:
                var randomInt = RandomNumberGenerator.GetInt32(0, neighbors.Count);
                goal = neighbors[randomInt];
                break;
            case PathBias.FurthestAway:
                var furthestNeighbour = neighbors.OrderBy(neighbourElement => (levelState.GetCharacterWithId(mTarget!)!.Position.ToGrid() - neighbourElement).Length()).Last();
                goal = furthestNeighbour;
                break;
            case PathBias.BehindNextHealthiestUnit:
                var charactersInRange = levelState.QuadTree.SearchCharacters(new Rect(
                    new Vector2(
                        mEnemy.Position.X - mEnemy.Range / 2,
                        mEnemy.Position.Y - mEnemy.Range / 2),
                    new Vector2(mEnemy.Range,
                        mEnemy.Range))).Where(c => !c.IsFriendly).ToList();

                charactersInRange.Remove(mEnemy);
                // currentLifePoints or MaxLifePoints
                if (charactersInRange.Count > 0)
                {
                    var characterWithMostHp = charactersInRange.OrderBy(c => c.CurrentLifePoints).First();
                    var characterWithMostHpNeighbours = levelState.GameMap.PassableNeighbors(characterWithMostHp.Position.ToGrid()).ToList();
                    var spotBehindCharacterWithMostHp = characterWithMostHpNeighbours.OrderBy(neighbourElement => (mEnemy.Position - neighbourElement).Length()).First();
                    goal = spotBehindCharacterWithMostHp;
                }
                else
                {
                    /*goal = closestNeighbor;*/
                }
                break;
        }
        gameLogic.MoveCharacter(mEnemy, Camera.TileCenterToWorld(goal));
    }
}