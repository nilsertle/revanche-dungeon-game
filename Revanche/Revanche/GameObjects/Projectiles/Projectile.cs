using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Revanche.Core;
using Revanche.Extensions;
using Revanche.Managers;

namespace Revanche.GameObjects.Projectiles
{
    public abstract class Projectile : GameObject
    {
        private const float ProjectileLayer = 0.5f;
        private const float F2 = 2f;
        private const float F3 = 3f;

        [JsonProperty] public ProjectileType Type { get; protected set; }
        [JsonProperty] public Vector2 Destination { get; protected set; }
        [JsonProperty] public float Velocity { get; protected set; }
        [JsonProperty] public int Damage { get; protected set; }
        
        [JsonProperty] public ProjectileEffect Effect { get; protected set; }
        [JsonProperty] public float Angle { get; protected set; }
        [JsonProperty] public bool IsFriendly { get; private set; }
        [JsonProperty] public string CharacterId { get; private set; }
        [JsonProperty] public ElementType Element { get; set; }
        [JsonProperty] public SoundEffects mSpawnSound;
        [JsonProperty] public SoundEffects mImpactSound;

        protected Projectile(Vector2 position, Vector2 destination, float velocity, bool isFriendly, string characterId, int damage, ElementType element) : base(position)
        {
            Destination = destination;
            Velocity = velocity;
            Angle = GetAngle();
            CurrentSpriteId = SpriteId;
            IsFriendly = isFriendly;
            CharacterId = characterId;
            Damage = damage;
            Element = element;
            LayerDepth = ProjectileLayer;
        }
        public new Rectangle Hitbox => new(
            (int)(Position.X - Game1.sScaledPixelSize / F3) + 1,
            (int)(Position.Y - Game1.sScaledPixelSize / F3) + 1,
            (int)(Game1.sScaledPixelSize * (F2 / F3) - 1),
            (int)(Game1.sScaledPixelSize * (F2 / F3) - 1));

        public override void Update(float deltaTime)
        {
            UpdateAnimation(deltaTime);
            var directionalVector = (Destination - Position);
            directionalVector.Normalize();
            var moveDistance = deltaTime * Velocity * directionalVector;
            if (Vector2.Distance(Position, Destination) > moveDistance.Length())
            {
                Position += moveDistance;
                return;
            }
            State = InstanceState.LimitReached;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.mDebugMode)
            {
                spriteBatch.DrawRectangleOutline(Hitbox, 1, Color.Red);
            }

            spriteBatch.Draw(AssetManager.mSpriteSheet,
                Position,
                AssetManager.GetRectangleFromId16(SpriteId + mCurrentFrame),
                Color.White,
                Angle,
                Game1.mOrigin,
                Game1.mScale,
                SpriteEffects.None,
                LayerDepth);
        }

        private float GetAngle()
        {
            var v1 = -Vector2.UnitX;
            var v2 = (Destination - Position);
            v1.Normalize();
            v2.Normalize();
            return (float)Math.Atan2(v1.X * v2.Y - v1.Y * v2.X, v1.X * v2.X + v1.Y * v2.Y);
        }
    }
}