using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.Projectiles
{
    public class WaterDroplet : Projectile
    {
        private const int Sprite = 2768;

        private const float F4 = 4f;

        public WaterDroplet(Vector2 position, Vector2 destination, bool isFriendly, string characterId, int damage, ElementType element) : base(position, destination, F4 * Game1.sScaledPixelSize, isFriendly, characterId, damage, element)
        {
            SpriteId = Sprite;
            Type = ProjectileType.WaterDroplet;
            mSpawnSound = SoundEffects.WaterDropletSpawn;
            mImpactSound = SoundEffects.WaterDropletImpact;
            Effect = ProjectileEffect.None;
        }
    }
}
