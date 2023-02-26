using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.Projectiles;

public class HealSpell : Projectile
{
    private const int Sprite = 2808;

    private const float F4 = 4f;

    public HealSpell(Vector2 position, Vector2 destination, bool isFriendly, string characterId, int damage, ElementType element) 
        : base(position, destination, F4 * Game1.sScaledPixelSize, isFriendly, characterId, damage, element)
    {
        SpriteId = Sprite;
        Type = ProjectileType.HealSpell;
        mSpawnSound = SoundEffects.HealingSpellSpawn;
        mImpactSound = SoundEffects.HealingSpellImpact;
        Effect = ProjectileEffect.Healing;
    }
}