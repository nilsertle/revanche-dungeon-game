namespace Revanche.Core
{

    /// <summary>
    /// SummonType used to specify the summon mode. Each enum type contains its
    /// SpriteId to draw the icon atop the Summoner
    /// </summary>
    public enum SummonType
    {
        Demon = 128,
        Skeleton = 320,
        StormCloud = 512,
        WaterElemental = 704,
        MagicSeedling = 896
    }

    public enum EnemyType
    {
        ConanTheBarbarian = 1152,
        FrontPensioner = 1472,
        Paladin = 1792,
        BombMagician = 2432,
        Pirate = 2112,
        ArchEnemy = 1088
    }

    public enum ElementIcon
    {
        Fire = 3086,
        Ghost = 3095,
        Lightning = 3103,
        Water = 3111,
        Magic = 3119
    }

    public enum Animations
    {
        IdleAnimation = 0,
        FancyIdleAnimation = 8,
        WalkingAnimation = 16,
        AttackAnimation = 24
    }

    public enum SoundEffects
    {
        PowerUpSound,
        BombSound,
        GameStart,
        MeleeAlliedHit,
        MeleeEnemyHit,
        BarClick,
        ButtonClick,
        Movement,
        InvalidAction,
        ChestOpening,
        GameLost,
        GameWon,
        PickupSoul,
        UpgradesConfirmed,
        GenericSummoning,
        UsePotion,
        LevelUp,
        HitWall,
        DestructibleHit,
        DestructibleBreaks,
        SummonerHit,
        ConanSpecial,

        DemonDeath,
        SkeletonDeath,
        StormCloudDeath,
        WaterElementalDeath,
        SeedlingDeath,
        SummonerDeath,

        ConanDeath,
        PensionerDeath,
        PaladinDeath,
        PirateDeath,
        BombMagicianDeath,
        ArchEnemyDeath,

        FireBallSpawn,
        SpeedSpellSpawn,
        HealingSpellSpawn,
        WaterDropletSpawn,
        StickSpawn,
        SulfurChunkSpawn,
        GunShotSpawn,
        BearTrapSpawn,

        FireBallImpact,
        SpeedSpellImpact,
        HealingSpellImpact,
        WaterDropletImpact,
        StickImpact,
        SulfurChunkImpact,
        GunShotImpact,
        BearTrapImpact
    }

    public enum Songs
    {
        Roaming,
        BossFight,
        BarClick,
        NoFade
    }

    public enum CombatType
    {
        Melee,
        AoE,
        Ranged,
        SelfDamageAoE
    }

    public enum CharacterState
    {
        Idle,
        Patrolling,
        Attacking,
        Fleeing,
        PlayerControl,
        ArchEnemyControl,
        Kiting
    }

    public enum ItemEffects
    {
        HealingEffect,
        SpeedEffect,
        TimedHealingEffect,
        TimedSpeedUpEffect,
        TimedDamageUpEffect,
    }

    public enum InstanceState
    {
        Pending,
        Paused,
        LimitReached
    }

    public enum DropAbleLoot
    {
        Soul,
        HealthPotion,
        DamagePotion,
        SpeedPotion, 
        Nothing
    }

    public enum MovementState
    {
        Idle,
        Moving
    }

    public enum EnvironmentalAnimations
    {
        ShrineHintMap = 21,
        OpenChest = 3008,
        BloodStain = 3024,
        BearTrapSnapping = 3016,
        BloodFountainTop = 3032,
        BloodFountainBottom = 3040,
        FireIcon = 3080,
        GhostIcon = 3088,
        LightningIcon = 3096,
        WaterIcon = 3104,
        MagicIcon = 3112
    }

    /// <summary>
    /// Places a visual object in the map. Modes: 
    /// SingleSprite: Places Sprite. 
    /// SingleAnimation: Place animation that plays once, then vanishes. 
    /// AnimationWithStop: Place animation that plays once, then stays at last frame.  
    /// FullAnimation: Place animation that loops and stays indefinitely.
    /// </summary>
    public enum EnvironmentalMode
    {
        SingleSprite,
        SingleAnimation,
        AnimationWithStop,
        FullAnimation
    }

    public enum VolumeMode
    {
        ChangeSoundVolume,
        ChangeMusicVolume
    }

    public enum SpellIcon
    {
        FireBall = 2816,
        SpeedSpell = 2824,
        HealingSpell = 2832
    }

    public enum Language
    {
        German,
        English
    }

    public enum TranslatedAssetTypes
    {
        // Main menu buttons
        NewGameButton,
        LoadButton,
        AchievementButton,
        TrainingButton,
        OptionsButton,
        ControlsButton,
        CreditsButton,
        ExitButton,

        // Control menu buttons
        Selection1Button,
        Selection2Button,
        CameraButton,
        MovementButton,
        PauseButton,
        InteractionButton,
        AttackButton,
        SummoningButton,
        SpellButton,
        TalentsButton,

        // Other menu buttons
        ReturnButton,
        DeleteButton,
        StatisticsButton,
        DemoButton,
        GermanButton,
        EnglishButton,
        ContinueButton,
        SaveButton,
        MainMenuButton,

        // Control menu screens
        Selection1Screen,
        Selection2Screen,
        CameraScreen,
        MovementScreen,
        PauseScreen,
        InteractionScreen,
        AttackScreen,
        SummoningScreen,
        SpellScreen,
        TalentsScreen,

        // Other screens
        AchievementScreen,
        StatisticsScreen,
        OptionsScreen,
        ControlsScreen,
        CreditsScreen,
        GameOverScreen,
        GameWonScreen,
    }
}