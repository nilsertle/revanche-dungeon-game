using Revanche.GameObjects;
using Revanche.GameObjects.Items;
using Revanche.GameObjects.Projectiles;
using Revanche.Managers;
using Revanche.Sound;
using System.Linq;
using Microsoft.Xna.Framework;
using Revanche.Extensions;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Revanche.GameObjects.Environment;
using Revanche.GameObjects.FriendlyUnits;

namespace Revanche.Core;

public sealed class CollisionHandler
{
    private const int MainCharacters = 2;
    private const int TunerLow = 3;
    private const int TunerLowMid = 5;
    private const int TunerMidHigh = 7;
    private const int TunerHigh = 10;

    private const float ThresholdLow = 0.1f;
    private const float ThresholdMid = 0.3f;
    private const float ThresholdHigh = 0.6f;


    private readonly LevelState mLevelState;
    private readonly EventDispatcher mEventDispatcher;
    private readonly CombatHandler mCombatHandler;
    private int mPerformeter;
    private int mNextStart;
    private int mTuner;
    public CollisionHandler(LevelState levelState, EventDispatcher eventDispatcher, CombatHandler combatHandler)
    {
        mLevelState = levelState;
        mEventDispatcher = eventDispatcher;
        mCombatHandler = combatHandler;
        mPerformeter = 0;
        mNextStart = 1;
        mTuner = TunerHigh;
    }

    public void HandleCollisions(float deltaTime)
    {
        mLevelState.ActionForCharacters(character => OnEachCollidingObject(character, deltaTime));
        if (!mLevelState.InTechDemo) //remove if, if you want the performance boost outside of the TD
        {
            return;
        }
        mPerformeter = mNextStart;
        mNextStart = (mNextStart + 1) % mTuner;
        if (mNextStart == 0)
        {
            UpdateTuner();
        }
    }

    private void UpdateTuner()
    {
        var totalChars = mLevelState.FriendlySummons.Count + mLevelState.HostileSummons.Count + 2; //Yes, I count the AE, even if he is defeated
        var movingChars = mLevelState.FriendlySummons.Values.Where(summon => summon != null).Count(summon => summon.Path.Count != 0);
        movingChars += mLevelState.HostileSummons.Values.Where(summon => summon != null).Count(summon => summon.Path.Count != 0);
        movingChars += MainCharacters;

        mTuner = (movingChars / (float)totalChars) switch
        {
            < ThresholdLow => TunerLow,
            < ThresholdMid => TunerLowMid,
            < ThresholdHigh => TunerMidHigh,
            _ => TunerHigh
        };
    }

    private void OnEachCollidingObject(Character character, float deltaTime)
    {
        if (mLevelState.InTechDemo) // make unconditional if you want the performance boost outside of the TD
        {
            this.mPerformeter = (this.mPerformeter + 1) % mTuner;

            if (this.mPerformeter != 0)
            {
                return;
            }
        }
        var collidingObjects = mLevelState.QuadTree.Search(character.Hitbox);
        collidingObjects.AddRange(mLevelState.MapTree.Search(character.Hitbox));
        foreach (var obj in collidingObjects.Where(obj => obj.Id != character.Id))
        {
            switch (obj)
            {
                case Character collidingCharacter:
                    HandleCharacterCollision(character, collidingCharacter, deltaTime);
                    break;
                case Projectile projectile:
                    HandleProjectileCollision(character, projectile);
                    break;
                case Soul soul:
                    HandleSoulCollision(character, soul);
                    break;
                case WallObject wall:
                    HandleWallCollision(character, wall, deltaTime);
                    break;
                case TreasureChest treasureChest:
                    if (character is Summoner)
                    {
                        HandleTreasureChestCollision(treasureChest);
                    }
                    break;
            }
        }
    }
    private void HandleCharacterCollision(GameObject character, GameObject collidingCharacter, float deltaTime)
    {
        if (mLevelState.MapTree.Search(collidingCharacter.Hitbox).Any())
        {
            return;
        }
        var direction = (collidingCharacter.Position - character.Position);
        if (direction == Vector2.Zero)
        {
            direction = Vector2.One;
        }
        direction.Normalize();
        collidingCharacter.Position += deltaTime * direction * Game1.sScaledPixelSize;
    }

    private void HandleProjectileCollision(Character character, Projectile projectile)
    {
        float radius = 2 * Game1.sScaledPixelSize;
        if (!(character.IsFriendly ^ projectile.IsFriendly))
        {
            return;
        }
        if (character is Summoner && (projectile.Type == ProjectileType.HealSpell || projectile.Type == ProjectileType.SpeedSpell))
        {
            return;
        }
        if (character is Summoner && mLevelState.InTechDemo)
        {
            return;
        }
        if (!(character.IsFriendly && (projectile is SpeedSpell || projectile is HealSpell)))
        {
            character.SetColor(Color.Red);
        }
        character.CurrentLifePoints -= (int)(projectile.Damage * projectile.Element.ElementalEffectiveness(character.Element));
        GiveEffectSpell(character, projectile.Effect, radius);
        mEventDispatcher.SendAudioRequest(new SoundEvent(projectile.mImpactSound, character.Position));
        projectile.State = InstanceState.LimitReached;
        if (!character.IsFriendly && character.CurrentState is CharacterState.Idle or CharacterState.Patrolling or CharacterState.ArchEnemyControl)
        {
            mCombatHandler.SetTargetWithId(character.Id, projectile.CharacterId);
        }
        if (projectile is BearTrap)
        {
            var newItem = new EnvironmentalItem(projectile.Position, EnvironmentalAnimations.BearTrapSnapping,
                    EnvironmentalMode.SingleAnimation);
            mLevelState.AddToMutableUseAble(newItem);
        }
    }

    private void HandleSoulCollision(GameObject character, Item soul)
    {
        if (character.Id == mLevelState.Summoner.Id ||
            mLevelState.FriendlySummons.ContainsKey(character.Id))
        {
            mLevelState.PickUpSoul(soul);
            mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.PickupSoul, character.Position));
        }
    }

    private static void HandleWallCollision(GameObject character, GameObject wall, float deltaTime)
    {
        var direction2 = (character.Position - wall.Position);
        if (direction2 == Vector2.Zero)
        {
            direction2 = Vector2.One;
        }
        direction2.Normalize();
        character.Position += deltaTime * direction2 * Game1.sScaledPixelSize;
    }

    private void HandleTreasureChestCollision(TreasureChest treasureChest)
    {
        // Remove _ with chance only when it is used due to resharper errors
        foreach (var (type, _) in treasureChest.mPossibleLoot)
        {
            Item newItem;
                switch (type)
                {
                    case DropAbleLoot.Soul:
                        newItem = new Soul(new Vector2(treasureChest.Position.X + 1 * Game1.sScaledPixelSize, treasureChest.Position.Y));
                        mLevelState.AddItem(newItem);
                        break;
                    case DropAbleLoot.HealthPotion:
                        newItem = new HealthPotion(new Vector2(treasureChest.Position.X - 1 * Game1.sScaledPixelSize, treasureChest.Position.Y));
                        mLevelState.AddItem(newItem);
                        break;
                }
        }
        treasureChest.State = InstanceState.LimitReached;
        mLevelState.Summoner.StopMovement();
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ChestOpening, treasureChest.Position));
    }

    private void GiveEffectSpell(Character character, ProjectileEffect projectileEffect, float radius)
    {
        character.UseProjectileEffect(projectileEffect, mLevelState.Summoner.HealingStrength, mLevelState.Summoner.SpeedStrength);
        var friendsInRange = mLevelState.QuadTree.SearchCharacters(new Rect(
            new Vector2(
                character.Position.X - (radius / 2),
                character.Position.Y - (radius / 2)),
            new Vector2(radius,
                radius))).Where(c => c.IsFriendly).ToList();
        foreach (var friend in friendsInRange)
        {
            if (Vector2.Distance(character.Position, friend.Position) < radius 
                && character.Id != friend.Id 
                && friend.Id != mLevelState.Summoner.Id)
            {
                friend.UseProjectileEffect(projectileEffect, 
                    mLevelState.Summoner.HealingStrength,
                    mLevelState.Summoner.SpeedStrength);
            }
        }
    }
}
