using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.FriendlyUnits;

internal sealed class WaterElemental : Summon
{
    private const int HpBase = 480;
    private const int HpScale = 70;
    private const int DamageBase = 45;
    private const int DamageScale = 12;
    private const int DelayBase = 175;

    private const float DelayScale = -2.5f;
    private const float GridSpeedBase = 3.5f;
    private const float GridSpeedScale = 0.125f;
    private const float GridRange = 5f;
    private const float GridVision = 6f;

    public WaterElemental(Vector2 position, int level, bool friendly = true) : base(position, level, friendly)
    {
        Level = level;
        SpriteId = (int)SummonType.WaterElemental;
        CurrentSpriteId = SpriteId;
        mDeathSound = SoundEffects.WaterElementalDeath;
        MaxLifePoints = AddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = AddIntScaling(DamageBase, DamageScale);
        Delay = AddIntScaling(DelayBase, DelayScale);
        Velocity = AddFloatScaling(GridSpeedBase, GridSpeedScale) * Game1.sScaledPixelSize;
        Element = ElementType.Water;
        IsFriendly = friendly;
        CombatType = CombatType.Ranged;
        Range = GridRange * Game1.sScaledPixelSize;
        Vision = GridVision * Game1.sScaledPixelSize;
    }
}