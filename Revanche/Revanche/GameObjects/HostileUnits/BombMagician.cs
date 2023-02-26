using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Revanche.AI;
using Revanche.AI.HostileSummonBehaviour;
using Revanche.Core;

namespace Revanche.GameObjects.HostileUnits;

internal sealed class BombMagician : Summon, IEnemyBehaviour
{
    private const int Sprite = 2432;
    private const int HpBase = 300;
    private const int HpScale = 45;
    private const int DamageBase = 250;
    private const int DamageScale = 50;
    private const int DelayBase = 600;

    private const float GridSpeedBase = 3.8f;
    private const float GridSpeedScale = 0.4f;
    private const float GridRange = 1.5f;
    private const float GridVision = 6f;

    [JsonProperty] private IEnemyBehaviour mBehaviour;
    public BombMagician(Vector2 position, int level, bool friendly = false) : base(position, level, friendly)
    {
        Level = level;
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
        mDeathSound = SoundEffects.BombMagicianDeath;
        MaxLifePoints = AddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = AddIntScaling(DamageBase, DamageScale);
        Delay = DelayBase;
        Velocity = AddFloatScaling(GridSpeedBase, GridSpeedScale) * Game1.sScaledPixelSize;
        Element = ElementType.Magic;
        IsFriendly = friendly;
        CombatType = CombatType.SelfDamageAoE;
        Range = GridRange * Game1.sScaledPixelSize;
        Vision = GridVision * Game1.sScaledPixelSize;
        mBehaviour = new BombMagicianBehaviour(this);
        CurrentState = CharacterState.Idle;

        Timer = (int)(DelayBase * 0.8f);
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