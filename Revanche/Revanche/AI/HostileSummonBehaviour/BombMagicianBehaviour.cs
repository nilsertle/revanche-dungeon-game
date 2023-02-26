using System;
using System.Linq;
using System.Security.Cryptography;
using System.Timers;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Revanche.Core;
using Revanche.Extensions;
using Revanche.GameObjects;

namespace Revanche.AI.HostileSummonBehaviour
{
    internal sealed class BombMagicianBehaviour : EnemyBehaviour, IEnemyBehaviour
    {
        private const int TimerInterval = 3000;
        private const int IdleCycles = 60;
        private const float FleeHpThreshold = 0.4f;
        private const int RegenerationDuration = 3;
        private const int RegenerationStrength = 1;

        [JsonProperty] private int mWaitCounter;
        [JsonProperty] private bool mHealed;
        [JsonProperty] private Timer mTimer;
        [JsonProperty] private bool mCanFlee;
        public BombMagicianBehaviour(Character bombMagician)
        {
            mEnemy = bombMagician;
            mWaitCounter = 0;
            mHealed = false;
            mTimer = new Timer(TimerInterval);
            mTimer.Elapsed += TimerElapsed; 
            mCanFlee = false;
        }
        public void UpdateState(LevelState levelState, GameLogic gameLogic)
        {
            switch (mEnemy.CurrentState)
            {
                case CharacterState.Idle:
                    Idle(levelState, gameLogic, IdleCycles);
                    break;
                case CharacterState.Attacking:
                    Attacking(levelState, gameLogic);
                    break;
                case CharacterState.Patrolling:
                    Patrolling(levelState, gameLogic);
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

        private void Idle(LevelState levelState, GameLogic gameLogic, int cycles)
        {
            if (mWaitCounter >= cycles)
            {
                mWaitCounter = 0;
                mEnemy.CurrentState = CharacterState.Patrolling;
            }

            mWaitCounter++;

            if (FirstTargetInVision(levelState, mEnemy) != null)
            {
                mEnemy.CurrentState = CharacterState.Attacking;
                gameLogic.AttackCharacter(mEnemy, FirstTargetInVision(levelState, mEnemy));
            }

            if (levelState.AttackerToTarget.ContainsKey(mEnemy.Id))
            {
                mEnemy.CurrentState = CharacterState.Attacking;
            }

        }

        private void Patrolling(LevelState levelState, GameLogic gameLogic)
        {

            if (levelState.AttackerToTarget.ContainsKey(mEnemy.Id))
            {
                mEnemy.CurrentState = CharacterState.Attacking;
                return;
            }

            var firstTarget = FirstTargetInVision(levelState, mEnemy);
            if (firstTarget != null)
            {
                mEnemy.CurrentState = CharacterState.Attacking;
                gameLogic.AttackCharacter(mEnemy, firstTarget!);
                return;
            }

            SetRandomPath(levelState, mEnemy, gameLogic);
            mEnemy.CurrentState = CharacterState.Idle;
        }

        private void Attacking(LevelState levelState, GameLogic gameLogic)
        {
            if (mEnemy.CurrentLifePoints >= mEnemy.MaxLifePoints * FleeHpThreshold)
            {
                return;
            }
            // State transition to fleeing after hp is low enough
            if (!mTimer.Enabled)
            {
                mTimer.Start();
            }

            if (mCanFlee)
            {
                var rand = 0;
                if (levelState.GameMap.RoomTopLeftCornerList.Count - 1 > 0)
                {
                    rand = RandomNumberGenerator.GetInt32(0, levelState.GameMap.RoomTopLeftCornerList.Count - 1);
                }

                var goal = new Vector2((int)levelState.GameMap.RoomTopLeftCornerList[rand].X * Game1.sScaledPixelSize,
                    (int)levelState.GameMap.RoomTopLeftCornerList[rand].Y * Game1.sScaledPixelSize);
                gameLogic.MoveCharacter(mEnemy, goal);
                mTimer.Stop();
                mCanFlee = false;
                mEnemy.CurrentState = CharacterState.Fleeing;
            }
        }

        private void Fleeing()
        {
            if (!mHealed)
            {
                mEnemy.HealUp = TemporaryEffect.CreateTemporaryEffect(RegenerationDuration, RegenerationStrength);
                mHealed = true;
            }
            if (mEnemy.mMovementState == MovementState.Idle)
            {
                mEnemy.CurrentState = CharacterState.Idle;
            }
        }

        private static void SetRandomPath(LevelState levelState, Character character, GameLogic gameLogic)
        {
            if (character.mMovementState == MovementState.Moving)
            {
                return;
            }

            var gridCoordinates = character.Position.ToGrid();
            var neighbors = levelState.GameMap.PassableNeighbors(gridCoordinates).ToList();
            var randomInt = RandomNumberGenerator.GetInt32(0, neighbors.Count);
            // Don't forget that neighbors[randomInt] is already grid coordinates, but move enemy requires
            // the world position (obtainable via TileCenterToWorld)
            gameLogic.MoveCharacter(character, Camera.TileCenterToWorld(neighbors[randomInt]));
        }



        private Character FirstTargetInVision(LevelState levelState, Character character)
        {
            if (IsSummonerInVision(levelState, character))
            {
                return levelState.Summoner;
            }

            if (AnyInVision(levelState, character) != null)
            {
                return AnyInVision(levelState, character);
            }

            return null;
        }

        private static Character AnyInVision(LevelState levelState, Character character)
        {
            var charactersInVision = levelState.QuadTree.SearchCharacters(new Rect(
                new Vector2(
                    character.Position.X - character.Vision,
                    character.Position.Y - character.Vision),
                new Vector2(character.Vision * 2,
                    character.Vision * 2))).Where(c => c.IsFriendly).ToList();

            return charactersInVision.FirstOrDefault();
        }


        private static bool IsSummonerInVision(LevelState levelState, Character character)
        {
            var distToSummoner = Math.Sqrt(Math.Pow((character.Position.X - levelState.Summoner.Position.X), 2) + Math.Pow((character.Position.Y - levelState.Summoner.Position.Y), 2));
            return distToSummoner <= character.Vision && InWallLineOfSight(levelState, character, levelState.Summoner);
        }

        private static bool InWallLineOfSight(LevelState levelState, GameObject sender, GameObject goal)
        {
            var slope = (goal.Position.Y - sender.Position.Y) / (goal.Position.X - sender.Position.X);
            if (Math.Abs(slope) <= 1)
            {
                for (var i = 0; i < goal.Position.X - sender.Position.X - 1; i++)
                {
                    var first = new Vector2((int)(sender.Position.X + Game1.sScaledPixelSize * (i + 0.5) - 1),
                        (int)(sender.Position.Y + slope * (i + 0.5) * Game1.sScaledPixelSize - 1)).ToGrid();
                    var second = new Vector2((int)(sender.Position.X + Game1.sScaledPixelSize * (i + 0.5)),
                        (int)(sender.Position.Y + slope * (i + 0.5) * Game1.sScaledPixelSize)).ToGrid();

                    if (levelState.GameMap.Collidable[(int)first.X, (int)first.Y] ||
                        levelState.GameMap.Collidable[(int)second.X, (int)second.Y])
                    {
                        return false;
                    }
                }
                return true;
            }

            slope = (goal.Position.X - sender.Position.X) / (goal.Position.Y - sender.Position.Y);
            for (var i = 0; i < goal.Position.Y - sender.Position.Y - 1; i++)
            {
                var first = new Vector2((int)(sender.Position.X + slope * (i + 0.5) * Game1.sScaledPixelSize - 1),
                    (int)(sender.Position.Y + Game1.sScaledPixelSize * (i + 0.5) - 1)).ToGrid();
                var second = new Vector2((int)(sender.Position.X + slope * (i + 0.5) * Game1.sScaledPixelSize),
                    (int)(sender.Position.Y + Game1.sScaledPixelSize * (i + 0.5))).ToGrid();

                if (levelState.GameMap.Collidable[(int)first.X, (int)first.Y] ||
                    levelState.GameMap.Collidable[(int)second.X, (int)second.Y])
                {
                    return false;
                }
            }
            return true;
        }
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            mCanFlee = true;
        }
    }
}