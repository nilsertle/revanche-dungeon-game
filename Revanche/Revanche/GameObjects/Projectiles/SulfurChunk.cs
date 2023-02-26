using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.Projectiles;

public class SulfurChunk : Projectile
{
    private const int Sprite = 2760;

    private const float F4 = 4f;

    public SulfurChunk(Vector2 position, Vector2 destination, bool isFriendly, string characterId, int damage, ElementType element) : base(position, destination, F4 * Game1.sScaledPixelSize, isFriendly, characterId, damage, element)
    {
        SpriteId = Sprite;
        Type = ProjectileType.SulfurChunk;
        mSpawnSound = SoundEffects.SulfurChunkSpawn;
        mImpactSound = SoundEffects.SulfurChunkImpact;
        Effect = ProjectileEffect.None;
    }
}