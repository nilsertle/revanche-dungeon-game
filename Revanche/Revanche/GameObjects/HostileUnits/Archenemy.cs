#nullable enable
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.AI;
using Revanche.AI.HostileSummonBehaviour;
using Revanche.Core;

namespace Revanche.GameObjects.HostileUnits;

public sealed class Archenemy : MainCharacter, IEnemyBehaviour
{
    private const int Sprite = 1088;
    private const int Experience = 100;
    private const int Mana = 10;
    private const int HpBase = 2000;
    private const int HpScale = 500;
    private const int DamageBase = 60;
    private const int DamageScale = 8;
    private const int DelayBase = 110;

    private const float GridSpeedBase = 4f;
    private const float GridSpeedScale = 0.1f;
    private const float GridRange = 3f;
    private const float GridVision = 7f;

    private readonly IEnemyBehaviour mBehaviour;
    
    public Archenemy(Vector2 position, int unitLevel, IEnemyBehaviour? behaviour) : base(position)
    {
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
        mDeathSound = SoundEffects.ArchEnemyDeath;
        MaxXp = Experience;
        MaxMana = Mana;
        CurrentMana = Mana;
        MaxLifePoints = McAddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = McAddIntScaling(DamageBase, DamageScale);
        Delay = DelayBase;
        Velocity = McAddFloatScaling(GridSpeedBase, GridSpeedScale) * Game1.sScaledPixelSize;
        Range = GridRange * Game1.sScaledPixelSize;
        Vision = GridVision * Game1.sScaledPixelSize;
        Element = ElementType.Neutral;
        IsFriendly = false;
        CurrentState = CharacterState.Patrolling;
        CombatType = CombatType.Melee;

        Skills = new Dictionary<ElementType, int>
        {
            { ElementType.Magic, unitLevel },
            { ElementType.Fire, unitLevel },
            { ElementType.Ghost, unitLevel },
            { ElementType.Water, unitLevel },
            { ElementType.Lightning, unitLevel }
        };
        mBehaviour = behaviour ?? new ArchEnemyBehaviour(this);
    }

    public void UpdateState(LevelState levelState, GameLogic gameLogic)
    {
        mBehaviour.UpdateState(levelState, gameLogic);
    }

    public void Initialize(Character enemy)
    {
        mBehaviour.Initialize(enemy);
    }

    public CharacterState GetState()
    {
        return CurrentState;
    }
}