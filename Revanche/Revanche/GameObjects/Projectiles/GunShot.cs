using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.Projectiles;

public class GunShot : Projectile
{
    private const int Sprite = 2784;

    private const float F4 = 4f;

    public GunShot(Vector2 position, Vector2 destination, bool isFriendly, string characterId, int damage, ElementType element) : base(position, destination, F4 * Game1.sScaledPixelSize, isFriendly, characterId, damage, element)
    {
        SpriteId = Sprite;
        Type = ProjectileType.Gunshot;
        mSpawnSound = SoundEffects.GunShotSpawn;
        mImpactSound = SoundEffects.GunShotImpact;
        Effect = ProjectileEffect.None;
    }
}