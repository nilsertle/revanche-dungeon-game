using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Revanche.AI;
using Revanche.AI.HostileSummonBehaviour;
using Revanche.Core;

namespace Revanche.GameObjects.HostileUnits;

internal sealed class ConanTheBarbarian : Summon, IEnemyBehaviour
{
    private const int Sprite = 1152;
    private const int HpBase = 600;
    private const int HpScale = 60;
    private const int DamageBase = 80;
    private const int DamageScale = 7;
    private const int DelayBase = 200;
    private const int DelayScale = -1;

    private const float GridSpeedBase = 4.5f;
    private const float GridRange = 1f;
    private const float GridVision = 6f;
    

    [JsonProperty] private IEnemyBehaviour mBehaviour;
    public ConanTheBarbarian(Vector2 position, int level, bool friendly = false) : base(position, level, friendly)
    {
        Level = level;
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
        mDeathSound = SoundEffects.ConanDeath;
        MaxLifePoints = AddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = AddIntScaling(DamageBase, DamageScale);
        Velocity = GridSpeedBase * Game1.sScaledPixelSize;
        Delay = AddIntScaling(DelayBase, DelayScale);
        Element = ElementType.Fire;
        IsFriendly = friendly;
        CombatType = CombatType.Melee;
        Range = GridRange * Game1.sScaledPixelSize;
        Vision = GridVision * Game1.sScaledPixelSize;
        CurrentState = CharacterState.Patrolling;
        mBehaviour = new ConanBehaviour(this);
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