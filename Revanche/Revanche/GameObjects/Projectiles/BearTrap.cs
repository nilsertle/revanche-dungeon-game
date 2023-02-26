using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.Projectiles;

public class BearTrap : Projectile
{
    private const int Sprite = 2792;

    public BearTrap(Vector2 position, Vector2 destination, bool isFriendly, string characterId, int damage, ElementType element) : base(position, destination, 0f, isFriendly, characterId, damage, element)
    {
        SpriteId = Sprite;
        Type = ProjectileType.BearTrap;
        mSpawnSound = SoundEffects.BearTrapSpawn;
        mImpactSound = SoundEffects.BearTrapImpact;
        Angle = 0f;
        Effect = ProjectileEffect.None;
    }
}