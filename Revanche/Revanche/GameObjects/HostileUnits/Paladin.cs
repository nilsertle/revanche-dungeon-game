using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Revanche.AI;
using Revanche.AI.HostileSummonBehaviour;
using Revanche.Core;

namespace Revanche.GameObjects.HostileUnits;

internal sealed class Paladin : Summon, IEnemyBehaviour
{
    private const int Sprite = 1792;
    private const int HpBase = 800;
    private const int HpScale = 100;
    private const int DamageBase = 100;
    private const int DamageScale = 6;
    private const int DelayBase = 500;

    private const float GridSpeedBase = 2.5f;
    private const float GridRange = 1f;
    private const float GridVision = 5f;

    [JsonProperty] private IEnemyBehaviour mBehaviour;
    public Paladin(Vector2 position, int level, bool friendly = false) : base(position, level, friendly)
    {
        Level = level;
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
        mDeathSound = SoundEffects.PaladinDeath;
        MaxLifePoints = AddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = AddIntScaling(DamageBase, DamageScale);
        Velocity = GridSpeedBase * Game1.sScaledPixelSize;
        Delay = DelayBase;
        Element = ElementType.Lightning;
        IsFriendly = friendly;
        CombatType = CombatType.Melee;
        Range = GridRange * Game1.sScaledPixelSize;
        Vision = GridVision * Game1.sScaledPixelSize;
        CurrentState = CharacterState.Patrolling;
        mBehaviour = new PaladinBehaviour(this);
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