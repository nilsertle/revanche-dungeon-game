using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Revanche.AI;
using Revanche.AI.HostileSummonBehaviour;
using Revanche.Core;

namespace Revanche.GameObjects.HostileUnits;

internal sealed class Pirate : Summon, IEnemyBehaviour
{
    private const int Sprite = 2112;
    private const int HpBase = 500;
    private const int HpScale = 40;
    private const int DamageBase = 40;
    private const int DamageScale = 4;
    private const int DelayBase = 120;
    private const float DelayScale = -2.5f;

    private const float GridSpeedBase = 3.5f;
    private const float GridRange = 5f;
    private const float GridVision = 8f;

    [JsonProperty] private IEnemyBehaviour mBehaviour;
    public Pirate(Vector2 position, int level, bool friendly = false) : base(position, level, friendly)
    {
        Level = level;
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
        mDeathSound = SoundEffects.PirateDeath;
        MaxLifePoints = AddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = AddIntScaling(DamageBase, DamageScale);
        Velocity = GridSpeedBase * Game1.sScaledPixelSize;
        Delay = AddIntScaling(DelayBase, DelayScale);
        Element = ElementType.Water;
        IsFriendly = friendly;
        CombatType = CombatType.Ranged;
        /*
         Range is interpreted as the distance from the character in every direction
         */
        Range = GridRange * Game1.sScaledPixelSize; //Warum nicht 5 schreiben? -> um Verwirrung zu vermeiden (so wei� man dass es eigentlich 10 sind wenn man mit vision vergleicht)
        /*
         Vision is interpreted as: vision^2 tiles are inside the vision
        -> Vision ist also 2 * Range (das wuerde das gleiche Rechteck im Debug modus ergeben)
         */
        // Wie kommt man bei 2*10/2 auf 14?
        Vision = GridVision * Game1.sScaledPixelSize;
        CurrentState = CharacterState.Patrolling;
        mBehaviour = new PirateBehaviour(this);
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