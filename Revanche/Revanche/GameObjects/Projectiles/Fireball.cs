using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.Projectiles;

internal sealed class Fireball : Projectile
{
    private const int Sprite = 2752;

    private const float F4 = 4f;

    public Fireball(Vector2 position, Vector2 destination, bool isFriendly, string characterId, int damage, ElementType element) 
        : base(position, destination, F4*Game1.sScaledPixelSize, isFriendly, characterId, damage, element)
    {
        SpriteId = Sprite;
        Type = ProjectileType.Fireball;
        mSpawnSound = SoundEffects.FireBallSpawn;
        mImpactSound = SoundEffects.FireBallImpact;
        Damage = damage;
        Effect = ProjectileEffect.None;
    }
}