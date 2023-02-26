using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Revanche.AI;
using Revanche.AI.HostileSummonBehaviour;
using Revanche.Core;

namespace Revanche.GameObjects.HostileUnits;

internal sealed class FrontPensioner : Summon, IEnemyBehaviour
{
    private const int Sprite = 1472;
    private const int HpBase = 500;
    private const int HpScale = 45;
    private const int DamageBase = 35;
    private const int DamageScale = 7;
    private const int DelayBase = 100;
    private const int DelayScale = 5;

    private const float GridSpeedBase = 1.8f;
    private const float GridSpeedScale = -0.25f;
    private const float GridRange = 5f;
    private const float GridVision = 4f;

    [JsonProperty] private IEnemyBehaviour mBehaviour;
    public FrontPensioner(Vector2 position, int level, bool friendly = false) : base(position, level, friendly)
    {
        Level = level;
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
        mDeathSound = SoundEffects.PensionerDeath;
        MaxLifePoints = AddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = AddIntScaling(DamageBase, DamageScale);
        Delay = AddIntScaling(DelayBase, DelayScale);
        Velocity = AddFloatScaling(GridSpeedBase, GridSpeedScale) * Game1.sScaledPixelSize;
        Element = ElementType.Ghost;
        IsFriendly = friendly;
        CombatType = CombatType.Ranged;
        Range = GridRange * Game1.sScaledPixelSize;
        Vision = GridVision * Game1.sScaledPixelSize;
        CurrentState = CharacterState.Patrolling;
        mBehaviour = new FrontPensionerBehaviour(this);
    }

    public void UpdateState(LevelState levelState, GameLogic gameLogic)
    {
        mBehaviour.UpdateState(levelState, gameLogic);
    }

    public void Initialize(Character enemy)
    {
        mBehaviour.Initialize(enemy);
    }
}