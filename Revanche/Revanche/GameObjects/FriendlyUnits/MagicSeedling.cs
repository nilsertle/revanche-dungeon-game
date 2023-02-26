using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.FriendlyUnits;

internal sealed class MagicSeedling : Summon
{
    private const int HpBase = 500;
    private const int HpScale = 180;
    private const int DamageBase = 35;
    private const int DamageScale = 10;
    private const int DelayBase = 150;
    
    private const float GridSpeedBase = 3.5f;
    private const float GridSpeedScale = 0.125f;
    private const float GridRange = 1f;
    private const float GridVision = 5f;

    public MagicSeedling(Vector2 position, int level, bool friendly = true) : base(position, level, friendly)
    {
        Level = level;
        SpriteId = (int)SummonType.MagicSeedling;
        CurrentSpriteId = SpriteId;
        mDeathSound = SoundEffects.SeedlingDeath;
        MaxLifePoints = AddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = AddIntScaling(DamageBase, DamageScale + this.Level);
        Delay = DelayBase;
        Velocity = AddFloatScaling(GridSpeedBase, GridSpeedScale) * Game1.sScaledPixelSize;
        Element = ElementType.Magic;
        IsFriendly = friendly;
        CombatType = CombatType.AoE;
        Range = GridRange * Game1.sScaledPixelSize;
        Vision = GridVision * Game1.sScaledPixelSize;
    }
}