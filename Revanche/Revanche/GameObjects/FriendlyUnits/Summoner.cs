using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Revanche.Core;
using Revanche.Managers;

namespace Revanche.GameObjects.FriendlyUnits;

public sealed class Summoner : MainCharacter
{
    [JsonProperty] public int FireballDamage { get; private set; } = 50;
    [JsonProperty] public int HealingStrength { get; private set; } = 1;
    [JsonProperty] public int SpeedStrength { get; private set; } = 1;
    
    [JsonProperty] public float FireBallCoolDown { get; set; }
    [JsonProperty] public bool EnoughManaFireBall { get; set; } = true;
    private const int FireBallCost = 20;
    public const int CoolDownLimitFire = 1;
    
    [JsonProperty] public float HealingSpellCoolDown { get; set; }
    [JsonProperty] public bool EnoughHealthHealing { get; set; } = true;
    public const int CoolDownLimitHeal = 2;
    
    [JsonProperty] public float SpeedSpellCoolDown { get; set; }
    [JsonProperty] public bool EnoughManaSpeed { get; set; } = true;
    private const int SpeedSpellCost = 50;
    public const int CoolDownLimitSpeed = 3;

    private const int Sprite = 64;
    private const int XpBase = 100;
    private const int ManaBase = 100;
    private const int ManaScale = 5;
    private const int HpBase = 666;
    private const int HpScale = 30;
    private const int DamageBase = 45;
    private const int DamageScale = 5;
    private const int DelayBase = 110;
    private const int FireBallDamageBase = 50;
    private const int FireBallDamageScale = 5;

    private const float GridSpeedBase = 5f;
    private const float GridRange = 1f;
    private const float GridVision = 7f;
    private const float SummonExtend = 3.5f;

    private const int I2 = 2;

    private const float SkillPassiveManaRegeneration = 0.015f;
    private const float F05 = 0.5f;
    private const float F08 = 0.8f;
    private const float BaseManaRegeneration = 0.08f;

    [JsonProperty] public int Souls { get; set; }
    [JsonProperty] public int SkillPoints { get; set; }
    [JsonProperty] public SummonType? SelectedSummonType { get; set; } // = null; // by default
    [JsonProperty] public float ActualMana { get; set; }
    [JsonProperty] public float SummonRange { get; set; }
    [JsonProperty] public float HealHpCost { get; set; } = 0.18f;
    public Summoner(Vector2 position) : base(position)
    {
        Skills = new Dictionary<ElementType, int>
        {
            { ElementType.Fire, 1 },
            { ElementType.Ghost, 1 },
            { ElementType.Lightning, 1 },
            { ElementType.Water, 1 },
            { ElementType.Magic, 1 }
        };
        UpdateScalingStats();
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
        mDeathSound = SoundEffects.SummonerDeath;
        AccumulatedXp = 0;
        CurrentMana = MaxMana;
        ActualMana = MaxMana;
        CurrentLifePoints = MaxLifePoints;
        Delay = DelayBase;
        Velocity = GridSpeedBase * Game1.sScaledPixelSize;
        Range = GridRange * Game1.sScaledPixelSize;
        Vision = GridVision * Game1.sScaledPixelSize;
        Element = ElementType.Neutral;
        Souls = 0;
        IsFriendly = true;
        SummonRange = SummonExtend;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (SelectedSummonType == null)
        {
            return;
        }
        spriteBatch.Draw(AssetManager.mSpriteSheet,
            Position + new Vector2(0, -Game1.sScaledPixelSize * F08),
            AssetManager.GetRectangleFromId16((int)SelectedSummonType + mCurrentFrame),
            Color.White,
            0f,
            Game1.mOrigin,
            Game1.mScale * F05, 
            SpriteEffects.None, // removed base
            0f);
    }

    public bool CanSpawnCharacterInRange(Vector2 id, int range)
    {
        var rec = new Rectangle((int)Position.X - range, (int)Position.Y - range, I2*range, I2*range);
        return rec.Contains(id);

    }

    public bool CanSummon()
    {
        return SelectedSummonType switch
        {
            SummonType.Demon => Souls >= 1,
            SummonType.Skeleton => Souls >= 1,
            SummonType.MagicSeedling => Souls >= 1,
            SummonType.StormCloud => Souls >= 1,
            SummonType.WaterElemental => Souls >= 1,
            _ => false
        };
    }

    public bool AddExp(int exp)
    {
        AccumulatedXp += exp;
        if (AccumulatedXp < MaxXp)
        {
            return false;
        }
        AccumulatedXp -= MaxXp;
        XpLevel++;
        SkillPoints++;
        UpdateScalingStats();
        return true;
    }

    public void UpdateSkills(Dictionary<ElementType, int> newSkills, int newSkillPoints)
    {
        Skills = newSkills;
        SkillPoints = newSkillPoints;
        UpdateScalingStats();
    }

    private void UpdateScalingStats()
    {
        MaxXp = McAddIntScaling(XpBase, 40 + 8 * XpLevel);
        MaxMana = McAddIntScaling(ManaBase, ManaScale) + (Skills[ElementType.Ghost] - 1) * ManaScale;
        MaxLifePoints = McAddIntScaling(HpBase, HpScale);
        CurrentLifePoints += HpScale;
        Damage = McAddIntScaling(DamageBase, DamageScale);
        FireballDamage = FireBallDamageBase + (Skills[ElementType.Fire] - 1) * FireBallDamageScale;
        HealingStrength = Skills[ElementType.Water];
        SpeedStrength = Skills[ElementType.Lightning];
    }

    internal void RegenerateMana()
    {
        ActualMana = Math.Clamp(ActualMana + (BaseManaRegeneration + SkillPassiveManaRegeneration * Skills[ElementType.Magic]), 0, MaxMana); // was (0.01f + 0.0001f * Skills[ElementType.Magic]), 0, MaxMana)
        CurrentMana = (int)Math.Floor(ActualMana);
    }

    internal void CheckEnoughManaSpell()
    {
        EnoughManaFireBall = !(ActualMana < FireBallCost);
        EnoughManaSpeed = !(ActualMana < SpeedSpellCost);
        EnoughHealthHealing = CurrentLifePoints >= (int)(MaxLifePoints * HealHpCost);
    }
}