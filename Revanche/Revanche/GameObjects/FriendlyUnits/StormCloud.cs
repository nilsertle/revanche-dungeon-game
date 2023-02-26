using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.FriendlyUnits;

internal sealed class StormCloud : Summon
{
    private const int HpBase = 300;
    private const int HpScale = 40;
    private const int DamageBase = 90;
    private const int DamageScale = 13;
    private const int DelayBase = 110;
    private const int DelayScale = -2;

    private const float GridSpeedBase = 5f;
    private const float GridSpeedScale = 0.1f;
    private const float GridRange = 1f;
    private const float GridVision = 5f;

    private const float AdditionalScaling = 1.5f;

    public StormCloud(Vector2 position, int level, bool friendly = true) : base(position, level, friendly)
    {
        Level = level;
        SpriteId = (int)SummonType.StormCloud;
        CurrentSpriteId = SpriteId;
        mDeathSound = SoundEffects.StormCloudDeath;
        MaxLifePoints = AddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = (int)AddFloatScaling(DamageBase, DamageScale + AdditionalScaling * this.Level);
        Delay = AddIntScaling(DelayBase, DelayScale);
        Velocity = AddFloatScaling(GridSpeedBase, GridSpeedScale * this.Level) * Game1.sScaledPixelSize;
        Element = ElementType.Lightning;
        IsFriendly = friendly;
        CombatType = CombatType.Melee;
        Range = GridRange * Game1.sScaledPixelSize;
        Vision = GridVision * Game1.sScaledPixelSize;
    }
}