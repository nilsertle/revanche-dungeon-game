using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.Projectiles;

public class SpeedSpell : Projectile
{
    private const int Sprite = 2800;

    private const float F4 = 4f;

    public SpeedSpell(Vector2 position, Vector2 destination, bool isFriendly, string characterId, int damage, ElementType element) 
        : base(position, destination, F4 * Game1.sScaledPixelSize, isFriendly, characterId, damage, element)
    {
        SpriteId = Sprite;
        Type = ProjectileType.SpeedSpell;
        mSpawnSound = SoundEffects.SpeedSpellSpawn;
        mImpactSound = SoundEffects.SpeedSpellImpact;
        Effect = ProjectileEffect.Speed;
    }
}