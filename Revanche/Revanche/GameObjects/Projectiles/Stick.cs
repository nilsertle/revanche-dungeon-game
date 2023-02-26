using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.Projectiles;

public class Stick : Projectile
{
    private const int Sprite = 2776;

    private const float F4 = 4f;

    public Stick(Vector2 position, Vector2 destination, bool isFriendly, string characterId, int damage, ElementType element) : base(position, destination, F4 * Game1.sScaledPixelSize, isFriendly, characterId, damage, element)
    {
        SpriteId = Sprite;
        Type = ProjectileType.Stick;
        mSpawnSound = SoundEffects.StickSpawn;
        mImpactSound = SoundEffects.StickImpact;
        Effect = ProjectileEffect.None;
    }
}