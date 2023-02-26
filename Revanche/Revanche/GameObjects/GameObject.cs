using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Revanche.Core;
using Revanche.Extensions;
using Revanche.Managers;

namespace Revanche.GameObjects;

public abstract class GameObject
{
    protected const int TotalFrames = 7;
    private const int FancyAnimationOffset = 8;
    private const int WalkAnimationOffset = 16;
    private const int AttackAnimationOffset = 24;

    protected const float TimeToNextFrame = 0.105f;
    private const float F2 = 2f;

    [JsonProperty] public Vector2 Position { get; set; }
    [JsonProperty] public int SpriteId { get; protected set; }
    [JsonProperty] public int CurrentSpriteId { get; protected set; }
    [JsonProperty] public InstanceState State { get; set; } = InstanceState.Pending;
    [JsonProperty] public string Id { get; protected set; } = Guid.NewGuid().ToString();
    [JsonProperty] public float LayerDepth { get; protected set; } = 0.1f;

    // Variables used for animation
    [JsonProperty] public bool mIsNotAnimated;
    [JsonProperty] public int mCurrentFrame;
    [JsonProperty] public Animations mCurrentAnimation;

    [JsonProperty] protected float mAnimationTimer;

    public Rectangle Hitbox => new(
            (int)(Position.X - Game1.sScaledPixelSize / F2)+1,
            (int)(Position.Y - Game1.sScaledPixelSize / F2)+1,
            (int)Game1.sScaledPixelSize-1,
            (int)Game1.sScaledPixelSize-1);
    
    protected GameObject(Vector2 position)
    {
        Position = position;
        CurrentSpriteId = SpriteId;
    }

    public void ChangeAnimation(Animations animation)
    {
        if (mIsNotAnimated || animation == mCurrentAnimation)
        {
            return;
        }
        switch (animation)
        {
            case Animations.IdleAnimation:
                CurrentSpriteId = SpriteId;
                mCurrentAnimation = Animations.IdleAnimation;
                break;
            case Animations.FancyIdleAnimation:
                CurrentSpriteId = SpriteId + FancyAnimationOffset;
                mCurrentAnimation = Animations.FancyIdleAnimation;
                break;
            case Animations.WalkingAnimation:
                CurrentSpriteId = SpriteId + WalkAnimationOffset;
                mCurrentAnimation = Animations.WalkingAnimation;
                break;
            case Animations.AttackAnimation:
                CurrentSpriteId = SpriteId + AttackAnimationOffset;
                mCurrentFrame = 0;
                mCurrentAnimation = Animations.AttackAnimation;
                break;
        }
    }

    protected void UpdateAnimation(float deltaTime)
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
            }
        }
        mAnimationTimer += deltaTime;
    }

    public virtual void Update(float deltaTime)
    {
        UpdateAnimation(deltaTime);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Game1.mDebugMode)
        {
            spriteBatch.DrawRectangleOutline(Hitbox, 1, Color.Red);
        }
        spriteBatch.Draw(AssetManager.mSpriteSheet,
        Position,
        AssetManager.GetRectangleFromId16(CurrentSpriteId + mCurrentFrame),
        Color.White,
        0f,
        Game1.mOrigin,
        Game1.mScale,
        SpriteEffects.None,
        LayerDepth);
    }
}