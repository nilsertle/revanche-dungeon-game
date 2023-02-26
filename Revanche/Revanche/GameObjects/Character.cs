using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Revanche.Managers;
using Revanche.Extensions;
using Revanche.GameObjects.Items;
using System;
using System.Linq;
using Revanche.Core;
using Revanche.GameObjects.Projectiles;

namespace Revanche.GameObjects;

public abstract class Character : GameObject
{
    private const int I2 = 2;
    private const int I4 = 4;
    private const int I10 = 10;
    private const int I25 = 25;
    private const int I30 = 30;
    private const int I32 = 32;
    private const int I35 = 35;
    private const int I50 = 50;
    private const int I64 = 64;
    private const int DamageUpIconId = 3136;
    private const int HealUpIconId = 3137;
    private const int SpeedUpIconId = 3138;

    private const int SoulChance = 30;
    private const int HealthPotionChance = 18;
    private const int DamagePotionChance = 8;
    private const int SpeedPotionChance = 4;
    private const int NothingChance = 40;

    private const float F01 = 0.1f;
    private const float F07 = 0.7f;
    private const float F15 = 1.5f;
    private const float F2 = 2f;
    private const float F4 = 4f;
    private const float MaxLevel = 5f;

    private const float F10 = 10f;

    [JsonProperty] public int MaxLifePoints { get; set; }
    [JsonProperty] public int CurrentLifePoints { get; set; }
    [JsonProperty] public int Damage { get; protected set; }
    [JsonProperty] public int Level { get; set; } = 1;
    [JsonProperty] public int DrawLevel { get; protected set; }
    [JsonProperty] public bool Selected { get; set; } //  = false; // by default
    [JsonProperty] public Vector2 Destination { get; set; }
    [JsonProperty] public ElementType Element { get; protected set; }
    // Chances in Percent, must add up to 100, else no drops at all
    [JsonProperty]
    public List<(DropAbleLoot type, int chance)> PossibleLoot { get; protected set; } = new() {(DropAbleLoot.Soul, SoulChance), (DropAbleLoot.HealthPotion, HealthPotionChance), (DropAbleLoot.DamagePotion, DamagePotionChance), (DropAbleLoot.SpeedPotion, SpeedPotionChance), (DropAbleLoot.Nothing, NothingChance)};
    [JsonProperty] public float Velocity { get; set; } = F4 * Game1.sScaledPixelSize;
    [JsonProperty] public Vector2 Direction { get; set; }
    [JsonProperty] public List<Vector2> Path { get; private set; } = new();
    [JsonProperty] private bool mIsFlipped;

    [JsonProperty] public MovementState mMovementState = MovementState.Idle;

    [JsonProperty] private Random mRandomInt = new ();

    [JsonProperty] private Color mColor = Color.White;

    [JsonProperty] private int mDelayCounter;

    [JsonProperty] public bool IsFriendly { get; protected set; }

    // ______________________________________________________
    [JsonProperty] public float Range { get; protected set; } = F4 * Game1.sScaledPixelSize;
    [JsonProperty] public float Vision { get; protected set; } = F10 * Game1.sScaledPixelSize;
    // Used for quad tree searches
    public Rectangle VisionRectangle => new((int)(Position.X - (Vision / I2)),(int)(Position.Y - (Vision / I2)), (int)Vision,(int)Vision);
    [JsonProperty] public CombatType CombatType { get; protected set; }
    [JsonProperty] public int Timer { get; set; } // 0 by default
    [JsonProperty] public int Delay { get; protected set; } = 120;
    // Damage dealt every 2 seconds (every 120 update steps)
    [JsonProperty] public TemporaryEffect HealUp { get; set; } = new ();
    [JsonProperty] public TemporaryEffect SpeedUp { get; set; } = new ();
    [JsonProperty] public TemporaryEffect DamageUp { get; set; } = new ();

    [JsonProperty] public CharacterState CurrentState { get; set; } = CharacterState.PlayerControl;
    [JsonProperty] public SoundEffects mDeathSound;
    [JsonProperty] private float mFancyAnimationTimer;
    [JsonProperty] private int mFancyAnimationLimit;


    protected Character(Vector2 position) : base(position)
    {
        Destination = position;
        Direction = Vector2.Zero;
        CurrentSpriteId = SpriteId;
        UpdateLevel(Level);
        LayerDepth = F07;
    }

    /// <summary>
    /// Can NOT be used for Main Characters! All other characters only.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="level"></param>
    /// <param name="friendly"></param>
    protected Character(Vector2 position, int level, bool friendly = false) : base(position)
    {
        Destination = position;
        Direction = Vector2.Zero;
        CurrentSpriteId = SpriteId;
        IsFriendly = friendly;
        UpdateLevel(level);
        LayerDepth = F07;
        mFancyAnimationLimit = (mRandomInt.Next() % 5 + 1) * 5;
    }

    /// <summary>
    /// Can NOT be used for Main Characters! All other characters only.
    /// </summary>
    /// <param name="level"></param>
    private void UpdateLevel(int level)
    {
        // Level = Math.Clamp(level, 1, 5);
        if (IsFriendly)
        {
            DrawLevel = (int)Math.Floor(level / MaxLevel * F2);
            return;
        }
        DrawLevel = level - 1;
    }

    private new void UpdateAnimation(float deltaTime)
    {
        if (mIsNotAnimated)
        {
            return;
        }
        if (mAnimationTimer > TimeToNextFrame)
        {
            mAnimationTimer = 0f;
            mCurrentFrame += 1;
            // Every animation consists out of 8 frames
            if (mCurrentFrame > TotalFrames)
            {
                mCurrentFrame = 0;
                if (mFancyAnimationTimer > mFancyAnimationLimit && CurrentState != CharacterState.Attacking && mCurrentAnimation == Animations.IdleAnimation)
                {
                    ChangeAnimation(Animations.FancyIdleAnimation);
                    mFancyAnimationTimer = 0f;
                    mFancyAnimationLimit = (mRandomInt.Next() % 5 + 1) * 5;
                }
                else
                {
                    ChangeAnimation(Animations.IdleAnimation);
                }
            }
        }
        mAnimationTimer += deltaTime;
        mFancyAnimationTimer += deltaTime;
    }
    public void SetPath(List<Vector2> path)
    {
        Path = path;
        mMovementState = MovementState.Moving;

        // Empty path and path == position
        if (path.Count <= 1)
        {
            return;
        }

        // Remove the start node from the path
        // We do this so the character doesn't
        // walk back to the start if it didn't
        // yet leave it's current grid square and
        // we recalculate a new path
        Path.RemoveAt(Path.Count - 1);
        Destination = Path[^1];
    }

    public void SetPosition(Vector2 position)
    {
        Path.Clear();
        Direction = Vector2.Zero;
        Position = position;
        Destination = position;
        mMovementState = MovementState.Idle;
    }

    public void StopMovement()
    {
        SetPosition(Position);
    }

    private void CalculateDirection()
    {
        var directionalVector = (Destination - Position);
        directionalVector.Normalize();
        Direction = directionalVector;
    }

    public override void Update(float deltaTime)
    {
        Heal();
        HealUp.Update(deltaTime);
        SpeedUp.Update(deltaTime);
        DamageUp.Update(deltaTime);
        UpdateAnimation(deltaTime);
        //Timer += 1;

        switch (mMovementState)
        {
            case MovementState.Idle:
                break;
            case MovementState.Moving:
                Move(deltaTime);
                break;
        }
    }

    private void Move(float deltaTime)
    {
        if (Path.Count > 0)
        {
            CalculateDirection();
            var moveDistance = deltaTime * (Velocity + SpeedUp.Active * SpeedUp.Strength * Game1.sScaledPixelSize) *
                               Direction;
            if (Vector2.Distance(Position, Destination) > moveDistance.Length())
            {
                ChangeAnimation(Animations.WalkingAnimation);
                mIsFlipped = Direction.X >= 0;
                Position += moveDistance;
                return;
            }

            Path.RemoveAt(Path.Count - 1);
            if (Path.Count == 0)
            {
                Position = Destination;
                Direction = Vector2.Zero;
                ChangeAnimation(Animations.IdleAnimation);
                mMovementState = MovementState.Idle;
                return;
            }
            Destination = Path[^1];
        }
    }

    private SpriteEffects GetSpriteEffect()
    {
        return mIsFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Game1.mDebugMode)
        {
            spriteBatch.DrawRectangleOutline(Hitbox, 1, Color.Red);
            foreach (Vector2 vector in Path)
            {
                var hitbox = new Rectangle((int)(vector.X - Game1.sScaledPixelSize / F2),
                  (int)(vector.Y - Game1.sScaledPixelSize / F2),
                  (int)Game1.sScaledPixelSize,
                  (int)Game1.sScaledPixelSize);

                spriteBatch.DrawRectangleOutline(hitbox, I4, Color.PowderBlue);
            }
            DrawCurrentEnemyAiState(spriteBatch, Color.LightBlue);
            spriteBatch.DrawRectangleOutline(new Rectangle((int)(Position.X - Vision / F2), (int)(Position.Y - Vision / F2), (int)Vision, (int)Vision), 2, Color.Green);
            spriteBatch.DrawRectangleOutline(new Rectangle((int)(Position.X - (Range / 1f)), (int)(Position.Y - (Range / 1f)), (int)(I2*Range), (int)(I2*Range)), 2, Color.IndianRed);
        }
        spriteBatch.Draw(AssetManager.mSpriteSheet,
            Position,
            AssetManager.GetRectangleFromId16((Selected ? CurrentSpriteId + I32 + DrawLevel * I64 : CurrentSpriteId + DrawLevel * I64) + mCurrentFrame),
            mColor,
            0f,
            Game1.mOrigin,
            Game1.mScale,
            GetSpriteEffect(),
            LayerDepth);
        DrawOverHeadStats(spriteBatch, Color.DarkRed, Color.DarkGreen);
        DrawBuffs(spriteBatch);
        if (mColor == Color.Red)
        {
            if (mDelayCounter > 20)
            {
                mColor = Color.White;
                mDelayCounter = 0;
            }
            else
            {
                mDelayCounter += 1;
            }
        }
    }

    private float GetLifeStatus()
    {
        var life = (float)CurrentLifePoints / MaxLifePoints;
        return life;
    }

    private void DrawBuffs(SpriteBatch spriteBatch)
    {
        if (DamageUp.Active == 1)
        {
            spriteBatch.Draw(AssetManager.mSpriteSheet,
                Position + new Vector2(-I25, -I50),
                AssetManager.GetRectangleFromId16(DamageUpIconId),
                Color.White,
                0f,
                Game1.mOrigin,
                Vector2.One * F15, 
                SpriteEffects.None,
                LayerDepth);
        }

        if (HealUp.Active == 1)
        {
            spriteBatch.Draw(AssetManager.mSpriteSheet,
                Position + new Vector2(0, -I50),
                AssetManager.GetRectangleFromId16(HealUpIconId),
                Color.White,
                0f,
                Game1.mOrigin,
                Vector2.One * F15,
                SpriteEffects.None,
                LayerDepth);
        }

        if (SpeedUp.Active == 1)
        {
            spriteBatch.Draw(AssetManager.mSpriteSheet,
                Position + new Vector2(I25, -I50),
                AssetManager.GetRectangleFromId16(SpeedUpIconId),
                Color.White,
                0f,
                Game1.mOrigin,
                Vector2.One * F15,
                SpriteEffects.None,
                LayerDepth);
        }
    }

    private void DrawOverHeadStats(SpriteBatch spriteBatch, Color color1, Color color2)
    {
        var life = GetLifeStatus();
        var pos = new Vector2(Position.X - I30, Position.Y + I35);
        var rect = new Rectangle((int)(Position.X - I30), (int)(Position.Y + I35), I50, I10);
        spriteBatch.Draw(AssetManager.mHudPaneBar, pos, rect, color1, 0f, Vector2.Zero, Vector2.One,
            SpriteEffects.None, LayerDepth); 
        spriteBatch.Draw(AssetManager.mHudPaneBar, pos,
            new Rectangle((int)(Position.X - I30), (int)(Position.Y + I35), (int)(life * I50), I10),
            color2, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, LayerDepth-F01);
    }

    private void DrawCurrentEnemyAiState(SpriteBatch spriteBatch, Color color1)
    {
        var currentStateString = CurrentState.ToString();
        var pos = new Vector2(Position.X - I30, Position.Y - I50);
        spriteBatch.DrawString(AssetManager.mHudFont, currentStateString, pos, color1, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
    }

    public void GetHit(float damage, float scaling)
    {
        CurrentLifePoints -= (int)(damage * scaling);
        mColor = Color.Red;
    }
    /// <summary>
    /// Iterates through the possible drops of itself. Returns an item with the set chance.
    /// </summary>
    public Item DropItem()
    {
        var pSum = this.PossibleLoot.Sum(loot => loot.chance);
        if (pSum != 100)
        {
            return null;
        }

        var rand = mRandomInt.Next();
        var curSum = 0;
        foreach (var loot in this.PossibleLoot)
        {
            curSum += loot.chance;
            if (rand % 100 >= curSum)
            {
                continue;
            }

            var lootPos = Camera.TileCenterToWorld(Position.ToGrid());
            Item outItem = loot.type switch
            {
                DropAbleLoot.Soul => new Soul(lootPos),
                DropAbleLoot.HealthPotion => new HealthPotion(lootPos),
                DropAbleLoot.DamagePotion => new DamagePotion(lootPos),
                DropAbleLoot.SpeedPotion => new SpeedPotion(lootPos),
                DropAbleLoot.Nothing => null,
                _ => null
            };
            return outItem;
        }
        return null;
        
    }

    private void Heal()
    {
        CurrentLifePoints = (int)Math.Clamp(CurrentLifePoints + (HealUp.Active * HealUp.Strength), -1, MaxLifePoints);
    }

    protected int AddIntScaling(int baseVal, float increment)
    {
        return (int)(baseVal + increment * (this.Level - 1));
    }

    protected float AddFloatScaling(float baseVal, float increment)
    {
        return baseVal + increment * (this.Level - 1);
    }

    public void SetColor(Color color)
    {
        mColor = color;
    }

    public void UseProjectileEffect(ProjectileEffect projectileEffect, int healingStrength, int speedStrength)
    {
        switch (projectileEffect)
        {
            case ProjectileEffect.None:
                break;
            case ProjectileEffect.Healing:
                HealUp = TemporaryEffect.CreateTemporaryEffect(I10, healingStrength);
                break;
            case ProjectileEffect.Speed:
                SpeedUp = TemporaryEffect.CreateTemporaryEffect(I10, speedStrength);
                break;
        }
    }
}
