using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.FriendlyUnits;

internal sealed class Demon : Summon
{
    private const int HpBase = 420;
    private const int HpScale = 50;
    private const int DamageBase = 30;
    private const int DamageScale = 7;
    private const int DelayBase = 100;

    private const float DelayScale = -2.5f;
    private const float GridSpeedBase = 4.5f;
    private const float GridRange = 3f;
    private const float GridVision = 5f;

    public Demon(Vector2 position, int level, bool friendly = true) : base(position, level, friendly)
    {
        Level = level;
        SpriteId = (int)SummonType.Demon;
        CurrentSpriteId = SpriteId;
        mDeathSound = SoundEffects.DemonDeath;
        MaxLifePoints = AddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = AddIntScaling(DamageBase, DamageScale);
        Delay = AddIntScaling(DelayBase, DelayScale);
        Velocity = GridSpeedBase * Game1.sScaledPixelSize;
        Element = ElementType.Fire;
        IsFriendly = friendly;
        CombatType = CombatType.Ranged;
        Range = GridRange * Game1.sScaledPixelSize;
        Vision = GridVision * Game1.sScaledPixelSize;
    }
}