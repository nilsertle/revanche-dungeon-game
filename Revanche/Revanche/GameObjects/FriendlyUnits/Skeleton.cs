using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.FriendlyUnits;

internal sealed class Skeleton : Summon
{
    private const int HpBase = 300;
    private const int HpScale = 75;
    private const int DamageBase = 45;
    private const int DamageScale = 10;
    private const int DelayBase = 78;
    private const int DelayScale = -2;

    private const float GridSpeedBase = 4f;
    private const float GridSpeedScale = 0.125f;
    private const float GridRange = 1f;
    private const float GridVision = 5f;
    public Skeleton(Vector2 position, int level, bool friendly = true) : base(position, level, friendly)
    {
        Level = level;
        SpriteId = (int)SummonType.Skeleton;
        CurrentSpriteId = SpriteId;
        mDeathSound = SoundEffects.SkeletonDeath;
        MaxLifePoints = AddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = AddIntScaling(DamageBase, DamageScale);
        Delay = AddIntScaling(DelayBase, DelayScale);
        Velocity = AddFloatScaling(GridSpeedBase, GridSpeedScale) * Game1.sScaledPixelSize;
        Element = ElementType.Ghost;
        IsFriendly = friendly;
        CombatType = CombatType.Melee;
        Range = GridRange * Game1.sScaledPixelSize;
        Vision = GridVision * Game1.sScaledPixelSize;
    }
}