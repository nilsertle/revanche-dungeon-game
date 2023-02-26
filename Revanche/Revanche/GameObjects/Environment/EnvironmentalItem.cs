using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Revanche.Core;

namespace Revanche.GameObjects.Environment
{
    sealed class EnvironmentalItem : GameObject
    {
        private const float EnvironmentalItemLayer = 0.9f;

        [JsonProperty] private bool mAnimationIsLooped;
        [JsonProperty] private bool mStayOnMap;
        [JsonProperty] public EnvironmentalAnimations mType;

        public EnvironmentalItem(Vector2 position, EnvironmentalAnimations type, EnvironmentalMode mode) : base(position)
        {
            mType = type;
            State = InstanceState.Pending;
            LayerDepth = EnvironmentalItemLayer;
            switch (mode)
            {
                case EnvironmentalMode.SingleSprite:
                    mIsNotAnimated = true;
                    mAnimationIsLooped = false;
                    mStayOnMap = true;
                    break;
                case EnvironmentalMode.SingleAnimation:
                    mIsNotAnimated = false;
                    mAnimationIsLooped = false;
                    mStayOnMap = false;
                    break;
                case EnvironmentalMode.AnimationWithStop:
                    mIsNotAnimated = false;
                    mAnimationIsLooped = false;
                    mStayOnMap = true;
                    break;
                case EnvironmentalMode.FullAnimation:
                    mIsNotAnimated = false;
                    mAnimationIsLooped = true;
                    mStayOnMap = true;
                    break;
            }
            SpriteId = (int)type;
            CurrentSpriteId = SpriteId;
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
                    if (mAnimationIsLooped)
                    {
                        mCurrentFrame = 0;
                    }
                    else
                    {
                        if (!mStayOnMap)
                        {
                            State = InstanceState.LimitReached;
                        }
                        mIsNotAnimated = true;
                        mCurrentFrame = TotalFrames;
                    }
                }
            }
            mAnimationTimer += deltaTime;
        }

        public override void Update(float deltaTime)
        {
            UpdateAnimation(deltaTime);
        }
    }
}
