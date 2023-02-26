using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Revanche.Core;

namespace Revanche.Managers;

public sealed class AssetManager
{
    internal static Texture2D mSpriteSheet;
    private readonly Dictionary<(TranslatedAssetTypes, Language), Texture2D> mTranslatedAssets = new();

    // Non-translated buttons
    internal readonly Texture2D mBaseButtonTexture;
    internal readonly Texture2D mBaseButtonRoundTexture;
    internal readonly Texture2D mDefaultButtonTexture;
    internal readonly Texture2D mSmallButtonTexture;
    internal readonly Texture2D mTinyButtonTexture;

    // Panes and backgrounds
    internal readonly Texture2D mPausePaneBackground;
    internal readonly Texture2D mMainMenuPaneBackground;
    internal readonly Texture2D mLoadPaneBackgroundTexture;

    internal readonly Texture2D mHudPaneBackground;
    internal static Texture2D mHudPaneBar;

    // Fonts
    internal static SpriteFont mHudFont;
    internal readonly SpriteFont mFont;
    
    //GameOver
    internal readonly Texture2D mGameOverScreenBackground;
    internal readonly Texture2D mGameWonScreenBackground;
    
    // Options
    internal readonly Texture2D mSoundButtonTexture;
    internal readonly Texture2D mSoundButtonTexture2;

    //Controls
    internal readonly Texture2D mAttackBackground;
    internal readonly Texture2D mSelectionBackground;
    internal readonly Texture2D mSummoningBackground;
    internal readonly Texture2D mMovementBackground;
    internal readonly Texture2D mInteractionBackground;
    internal readonly Texture2D mCameraBackground;
    internal readonly Texture2D mMagicBackground;
    internal readonly Texture2D mUpgradeBackground;
    internal readonly Texture2D mPauseBackground;
    internal readonly Texture2D mSkillsMenuBackground;
    internal readonly Texture2D mSelectionBackground2;

    // Sounds
    internal readonly SoundEffect mSoundDemonDeath;
    internal readonly SoundEffect mSoundSkeletonDeath;
    internal readonly SoundEffect mSoundStormCloudDeath;
    internal readonly SoundEffect mSoundWaterElementalDeath;
    internal readonly SoundEffect mSoundSeedlingDeath;
    internal readonly SoundEffect mSoundSummonerDeath;

    internal readonly SoundEffect mSoundConanDeath;
    internal readonly SoundEffect mSoundPensionerDeath;
    internal readonly SoundEffect mSoundPaladinDeath;
    internal readonly SoundEffect mSoundPirateDeath;
    internal readonly SoundEffect mSoundBombMagicianDeath;
    internal readonly SoundEffect mSoundArchEnemyDeath;

    internal readonly SoundEffect mSoundFireBallSpawn;
    internal readonly SoundEffect mSoundSpeedSpellSpawn;
    internal readonly SoundEffect mSoundHealingSpellSpawn;
    internal readonly SoundEffect mSoundSulfurChunkSpawn;
    internal readonly SoundEffect mSoundWaterDropletSpawn;
    internal readonly SoundEffect mSoundStickSpawn;
    internal readonly SoundEffect mSoundGunShotSpawn;
    internal readonly SoundEffect mSoundBearTrapSpawn;

    internal readonly SoundEffect mSoundFireBallImpact;
    internal readonly SoundEffect mSoundSpeedSpellImpact;
    internal readonly SoundEffect mSoundHealingSpellImpact;
    internal readonly SoundEffect mSoundSulfurChunkImpact;
    internal readonly SoundEffect mSoundWaterDropletImpact;
    internal readonly SoundEffect mSoundStickImpact;
    internal readonly SoundEffect mSoundGunShotImpact;
    internal readonly SoundEffect mSoundBearTrapImpact;

    internal readonly SoundEffect mSoundMovement;
    internal readonly SoundEffect mSoundPowerUp;
    internal readonly SoundEffect mSoundBomb;
    internal readonly SoundEffect mSoundGameStart;
    internal readonly SoundEffect mSoundBarClick;
    internal readonly SoundEffect mSoundButtonClick;
    internal readonly SoundEffect mSoundMeleeAlliedHit;
    internal readonly SoundEffect mSoundMeleeEnemyHit;
    internal readonly SoundEffect mSoundInvalidMovement;
    internal readonly SoundEffect mSoundChestOpening;
    internal readonly SoundEffect mSoundGameLost;
    internal readonly SoundEffect mSoundGameWon;
    internal readonly SoundEffect mSoundPickupSoul;
    internal readonly SoundEffect mSoundUpgradeConfirmed;
    internal readonly SoundEffect mSoundGenericSummoning;
    internal readonly SoundEffect mSoundUsePotion;
    internal readonly SoundEffect mSoundLevelUp;
    internal readonly SoundEffect mSoundWallHit;
    internal readonly SoundEffect mSoundDestructibleWallHit;
    internal readonly SoundEffect mSoundDestructibleWallBreaks;
    internal readonly SoundEffect mSoundSummonerGetsHit;
    internal readonly SoundEffect mSoundConanSpecial;

    internal readonly SoundEffect mMusicDungeonRoaming;
    internal readonly SoundEffect mMusicBossFight;

    //Demo
    internal readonly Texture2D mTechDemoBackground;
    internal AssetManager(ContentManager content)
    {
        // Main menu buttons - GER
        var newGameButtonTextureGerman = content.Load<Texture2D>("newGameButton");
        var loadButtonTextureGerman = content.Load<Texture2D>("loadButton");
        var achievementButtonTextureGerman = content.Load<Texture2D>("achievementButton");
        var trainingButtonTextureGerman = content.Load<Texture2D>("trainingButton");
        var optionButtonTextureGerman = content.Load<Texture2D>("optionButton");
        var controlsButtonTextureGerman = content.Load<Texture2D>("controlsButton");
        var creditsButtonGerman = content.Load<Texture2D>("CreditsButton");
        var exitButtonTextureGerman = content.Load<Texture2D>("exitButton");

        // Controls menu buttons - GER
        var selectionButton1German = content.Load<Texture2D>("controls_button_selection1");
        var selectionButton2German = content.Load<Texture2D>("controls_button_selection2");
        var cameraButtonGerman = content.Load<Texture2D>("controls_button_camera");
        var movementButtonGerman = content.Load<Texture2D>("controls_button_movement");
        var pauseButtonGerman = content.Load<Texture2D>("controls_button_pause");
        var interactionButtonGerman = content.Load<Texture2D>("controls_button_interaction");
        var attackButtonGerman = content.Load<Texture2D>("controls_button_attack");
        var summoningButtonGerman = content.Load<Texture2D>("controls_button_summoning");
        var spellButtonGerman = content.Load<Texture2D>("controls_button_spells");
        var talentsButtonGerman = content.Load<Texture2D>("controls_button_talents");

        // Other menu buttons - GER
        var returnButtonTextureGerman = content.Load<Texture2D>("returnButton");
        var deleteButtonGerman = content.Load<Texture2D>("Delete_Button");
        var statisticButtonGerman = content.Load<Texture2D>("Statistics_Button");
        var techDemoButtonTextureGerman = content.Load<Texture2D>("techDemoButton");
        var continueButtonTextureGerman = content.Load<Texture2D>("continueButton");
        var saveButtonTextureGerman = content.Load<Texture2D>("saveButton");
        var mainMenuButtonGerman = content.Load<Texture2D>("Main_Menu_Button");
        var germanButtonGerman = content.Load<Texture2D>("germanButton");
        var englishButtonGerman = content.Load<Texture2D>("englishButton");

        // Other screens - GER
        var achievementScreenBackgroundGerman = content.Load<Texture2D>("achievementBackgroundNew");
        var statisticsScreenBackgroundGerman = content.Load<Texture2D>("statsBackground");
        var optionsScreenBackgroundGerman = content.Load<Texture2D>("options");
        var controlsScreenBackgroundGerman = content.Load<Texture2D>("Control_Background");
        var creditsScreenBackgroundGerman = content.Load<Texture2D>("CreditScreen");


        // Main menu buttons - ENG
        var newGameButtonTextureEnglish = content.Load<Texture2D>("newGameButtonENG");
        var loadButtonTextureEnglish = content.Load<Texture2D>("loadButtonENG");
        var achievementButtonTextureEnglish = content.Load<Texture2D>("achievementButtonENG");
        var optionButtonTextureEnglish = content.Load<Texture2D>("optionButtonENG");
        var controlsButtonTextureEnglish = content.Load<Texture2D>("controlsButtonENG");
        var creditsButtonEnglish = content.Load<Texture2D>("CreditsButtonENG");
        var exitButtonTextureEnglish = content.Load<Texture2D>("exitButtonENG");

        // Controls menu buttons - ENG
        var selectionButton1English = content.Load<Texture2D>("controls_button_selection1ENG");
        var selectionButton2English = content.Load<Texture2D>("controls_button_selection2ENG");
        var cameraButtonEnglish = content.Load<Texture2D>("controls_button_cameraENG");
        var movementButtonEnglish = content.Load<Texture2D>("controls_button_movementENG");
        var pauseButtonEnglish = content.Load<Texture2D>("controls_button_pauseENG");
        var interactionButtonEnglish = content.Load<Texture2D>("controls_button_interactionENG");
        var attackButtonEnglish = content.Load<Texture2D>("controls_button_attackENG");
        var summoningButtonEnglish = content.Load<Texture2D>("controls_button_summoningENG");
        var spellButtonEnglish = content.Load<Texture2D>("controls_button_spellsENG");
        var talentsButtonEnglish = content.Load<Texture2D>("controls_button_talentsENG");

        // Other menu buttons - ENG
        var returnButtonTextureEnglish = content.Load<Texture2D>("returnButtonENG");
        var deleteButtonEnglish = content.Load<Texture2D>("Delete_ButtonENG");
        var statisticButtonEnglish = content.Load<Texture2D>("Statistics_ButtonENG");
        var continueButtonTextureEnglish = content.Load<Texture2D>("continueButtonENG");
        var saveButtonTextureEnglish = content.Load<Texture2D>("saveButtonENG");
        var mainMenuButtonEnglish = content.Load<Texture2D>("Main_Menu_ButtonENG");
        var germanButtonEnglish = content.Load<Texture2D>("germanButtonENG");
        var englishButtonEnglish = content.Load<Texture2D>("englishButtonENG");

        // Other screens - GER
        var achievementScreenBackgroundEnglish = content.Load<Texture2D>("achievementBackgroundNewENG");
        var statisticsScreenBackgroundEnglish = content.Load<Texture2D>("statsBackgroundENG");
        var optionsScreenBackgroundEnglish = content.Load<Texture2D>("optionsENG");
        var controlsScreenBackgroundEnglish = content.Load<Texture2D>("Control_BackgroundENG");
        var creditsScreenBackgroundEnglish = content.Load<Texture2D>("CreditScreenENG");

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        mGameOverScreenBackground = content.Load<Texture2D>("gameOverPanel");
        mGameWonScreenBackground = content.Load<Texture2D>("gamewon");

        mHudPaneBackground = content.Load<Texture2D>("SamplePane");
        mHudFont = content.Load<SpriteFont>("font");
        mSpriteSheet = content.Load<Texture2D>("AnimationSpriteSheet");

        mPausePaneBackground = content.Load<Texture2D>(assetName: "background2New2");

        mMainMenuPaneBackground = content.Load<Texture2D>("mainMenuBackground");

        mBaseButtonTexture = content.Load<Texture2D>("BaseButton");
        mBaseButtonRoundTexture = content.Load<Texture2D>("BaseButtonRoundNew");
        mLoadPaneBackgroundTexture = content.Load<Texture2D>("loadPaneBackground");
        mSoundButtonTexture = content.Load<Texture2D>("soundbar");
        mSoundButtonTexture2 = content.Load<Texture2D>("soundbar2");

        mUpgradeBackground = content.Load<Texture2D>("UpgradeScreenBackground");
        mSmallButtonTexture = content.Load<Texture2D>("smallButton");
        mTinyButtonTexture = content.Load<Texture2D>("tinyButton");

        mTechDemoBackground = content.Load<Texture2D>("techDemoBackground");

        mDefaultButtonTexture = content.Load<Texture2D>("defaultButton");
        mHudPaneBar = content.Load<Texture2D>(assetName: "lifebar");

        mAttackBackground =  content.Load<Texture2D>("controls_attack");
        mSelectionBackground = content.Load<Texture2D>("controls_selection1");
        mSelectionBackground2 = content.Load<Texture2D>("controls_selection2");
        mSummoningBackground = content.Load<Texture2D>("controls_summoning");
        mMovementBackground = content.Load<Texture2D>("controls_movement");
        mInteractionBackground = content.Load<Texture2D>("controls_interaction");
        mCameraBackground = content.Load<Texture2D>("controls_camera");
        mMagicBackground = content.Load<Texture2D>("controls_spells");
        mPauseBackground = content.Load<Texture2D>("controls_pause_game");
        mSkillsMenuBackground = content.Load<Texture2D>("controls_talents");

        mFont = content.Load<SpriteFont>("File");

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // Sounds
        mSoundDemonDeath = content.Load<SoundEffect>(assetName: "Demon_Death");
        mSoundSkeletonDeath = content.Load<SoundEffect>(assetName: "Skeleton_Death");
        mSoundStormCloudDeath = content.Load<SoundEffect>(assetName: "Storm_Cloud_Death");
        mSoundWaterElementalDeath = content.Load<SoundEffect>(assetName: "Water_Elemental_Death");
        mSoundSeedlingDeath = content.Load<SoundEffect>(assetName: "Seedling_Death");
        mSoundSummonerDeath = content.Load<SoundEffect>(assetName: "Summoner_Death");

        mSoundConanDeath = content.Load<SoundEffect>(assetName: "Conan_Death");
        mSoundPensionerDeath = content.Load<SoundEffect>(assetName: "Pensioner_Death");
        mSoundPaladinDeath = content.Load<SoundEffect>(assetName: "Paladin_Death");
        mSoundPirateDeath = content.Load<SoundEffect>(assetName: "Pirate_Death");
        mSoundBombMagicianDeath = content.Load<SoundEffect>(assetName: "Bomb_Magician_Death");
        mSoundArchEnemyDeath = content.Load<SoundEffect>(assetName: "Arch_Enemy_Death");

        mSoundFireBallSpawn = content.Load<SoundEffect>(assetName: "Fire_Ball_Spawn");
        mSoundSpeedSpellSpawn = content.Load<SoundEffect>(assetName: "Speed_Spell_Spawn");
        mSoundHealingSpellSpawn = content.Load<SoundEffect>(assetName: "Healing_Spell_Spawn");
        mSoundSulfurChunkSpawn = content.Load<SoundEffect>(assetName: "Sulfur_Chunk_Spawn");
        mSoundWaterDropletSpawn = content.Load<SoundEffect>(assetName: "Water_Droplet_Spawn");
        mSoundStickSpawn = content.Load<SoundEffect>(assetName: "Stick_Spawn");
        mSoundGunShotSpawn = content.Load<SoundEffect>(assetName: "Gun_Shot_Spawn");
        mSoundBearTrapSpawn = content.Load<SoundEffect>(assetName: "Bear_Trap_Spawn");

        mSoundFireBallImpact = content.Load<SoundEffect>(assetName: "Fire_Ball_Impact");
        mSoundSpeedSpellImpact = content.Load<SoundEffect>(assetName: "Speed_Spell_Impact");
        mSoundHealingSpellImpact = content.Load<SoundEffect>(assetName: "Healing_Spell_Impact");
        mSoundSulfurChunkImpact = content.Load<SoundEffect>(assetName: "Sulfur_Chunk_Impact");
        mSoundWaterDropletImpact = content.Load<SoundEffect>(assetName: "Water_Droplet_Impact");
        mSoundStickImpact = content.Load<SoundEffect>(assetName: "Stick_Impact");
        mSoundGunShotImpact = content.Load<SoundEffect>(assetName: "Gun_Shot_Impact");
        mSoundBearTrapImpact = content.Load<SoundEffect>(assetName: "Bear_Trap_Impact");

        mSoundMovement = content.Load<SoundEffect>(assetName: "Movement");
        mSoundPowerUp = content.Load<SoundEffect>(assetName: "Normal_power_up_01");
        mSoundBomb = content.Load<SoundEffect>(assetName: "Bomb_explosion_01");
        mSoundGameStart = content.Load<SoundEffect>(assetName: "Game_Start");
        mSoundBarClick = content.Load<SoundEffect>(assetName: "Generic_Bar_Click");
        mSoundButtonClick = content.Load<SoundEffect>(assetName: "Generic_Button_Click");
        mSoundMeleeAlliedHit = content.Load<SoundEffect>(assetName: "Melee_Allied_Strike");
        mSoundMeleeEnemyHit = content.Load<SoundEffect>(assetName: "Melee_Enemy_Strike");
        mSoundInvalidMovement = content.Load<SoundEffect>(assetName: "Invalid_Movement");
        mSoundChestOpening = content.Load<SoundEffect>(assetName: "Chest_Opening");
        mSoundGameLost = content.Load<SoundEffect>(assetName: "Lose_Game");
        mSoundGameWon = content.Load<SoundEffect>(assetName: "Game_Won");
        mSoundPickupSoul = content.Load<SoundEffect>(assetName: "Pickup_Soul");
        mSoundUpgradeConfirmed = content.Load<SoundEffect>(assetName: "Upgrades_Confirmed");
        mSoundGenericSummoning = content.Load<SoundEffect>(assetName: "Generic_Summoning");
        mSoundUsePotion = content.Load<SoundEffect>(assetName: "Use_Potion");
        mSoundLevelUp = content.Load<SoundEffect>(assetName: "XP_Level_Up");
        mSoundWallHit = content.Load<SoundEffect>(assetName: "Projectile_Hits_Wall");
        mSoundDestructibleWallHit = content.Load<SoundEffect>(assetName: "Destructible_Wall_Hit");
        mSoundDestructibleWallBreaks = content.Load<SoundEffect>(assetName: "Destructible_Wall_Destroyed");
        mSoundSummonerGetsHit = content.Load<SoundEffect>(assetName: "Summoner_Get_Hit");
        mSoundConanSpecial = content.Load<SoundEffect>(assetName: "Conan_Special");

        // Music
        mMusicDungeonRoaming = content.Load<SoundEffect>(assetName: "Dungeon_Boss_Low_Level_Wolves");
        mMusicBossFight = content.Load<SoundEffect>(assetName: "Dungeon_Boss_Polyglyphics");

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // Main menu buttons - GER
        mTranslatedAssets.Add((TranslatedAssetTypes.NewGameButton, Language.German), newGameButtonTextureGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.LoadButton, Language.German), loadButtonTextureGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.AchievementButton, Language.German), achievementButtonTextureGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.TrainingButton, Language.German), trainingButtonTextureGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.OptionsButton, Language.German), optionButtonTextureGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.ControlsButton, Language.German), controlsButtonTextureGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.CreditsButton, Language.German), creditsButtonGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.ExitButton, Language.German), exitButtonTextureGerman);

        // Controls menu buttons - GER
        mTranslatedAssets.Add((TranslatedAssetTypes.Selection1Button, Language.German), selectionButton1German);
        mTranslatedAssets.Add((TranslatedAssetTypes.Selection2Button, Language.German), selectionButton2German);
        mTranslatedAssets.Add((TranslatedAssetTypes.CameraButton, Language.German), cameraButtonGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.MovementButton, Language.German), movementButtonGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.PauseButton, Language.German), pauseButtonGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.InteractionButton, Language.German), interactionButtonGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.AttackButton, Language.German), attackButtonGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.SummoningButton, Language.German), summoningButtonGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.SpellButton, Language.German), spellButtonGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.TalentsButton, Language.German), talentsButtonGerman);

        // Other menu buttons - GER
        mTranslatedAssets.Add((TranslatedAssetTypes.ReturnButton, Language.German), returnButtonTextureGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.DeleteButton, Language.German), deleteButtonGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.StatisticsButton, Language.German), statisticButtonGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.DemoButton, Language.German), techDemoButtonTextureGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.ContinueButton, Language.German), continueButtonTextureGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.SaveButton, Language.German), saveButtonTextureGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.MainMenuButton, Language.German), mainMenuButtonGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.GermanButton, Language.German), germanButtonGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.EnglishButton, Language.German), englishButtonGerman);

        // Other screens - GER
        mTranslatedAssets.Add((TranslatedAssetTypes.AchievementScreen, Language.German), achievementScreenBackgroundGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.StatisticsScreen, Language.German), statisticsScreenBackgroundGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.OptionsScreen, Language.German), optionsScreenBackgroundGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.ControlsScreen, Language.German), controlsScreenBackgroundGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.CreditsScreen, Language.German), creditsScreenBackgroundGerman);


        // Main menu buttons - ENG
        mTranslatedAssets.Add((TranslatedAssetTypes.NewGameButton, Language.English), newGameButtonTextureEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.LoadButton, Language.English), loadButtonTextureEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.AchievementButton, Language.English), achievementButtonTextureEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.TrainingButton, Language.English), trainingButtonTextureGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.OptionsButton, Language.English), optionButtonTextureEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.ControlsButton, Language.English), controlsButtonTextureEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.CreditsButton, Language.English), creditsButtonEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.ExitButton, Language.English), exitButtonTextureEnglish);

        // Controls screen menu - ENG
        mTranslatedAssets.Add((TranslatedAssetTypes.Selection1Button, Language.English), selectionButton1English);
        mTranslatedAssets.Add((TranslatedAssetTypes.Selection2Button, Language.English), selectionButton2English);
        mTranslatedAssets.Add((TranslatedAssetTypes.CameraButton, Language.English), cameraButtonEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.MovementButton, Language.English), movementButtonEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.PauseButton, Language.English), pauseButtonEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.InteractionButton, Language.English), interactionButtonEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.AttackButton, Language.English), attackButtonEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.SummoningButton, Language.English), summoningButtonEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.SpellButton, Language.English), spellButtonEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.TalentsButton, Language.English), talentsButtonEnglish);

        // Other menu buttons - ENG
        mTranslatedAssets.Add((TranslatedAssetTypes.ReturnButton, Language.English), returnButtonTextureEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.DeleteButton, Language.English), deleteButtonEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.StatisticsButton, Language.English), statisticButtonEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.DemoButton, Language.English), techDemoButtonTextureGerman);
        mTranslatedAssets.Add((TranslatedAssetTypes.ContinueButton, Language.English), continueButtonTextureEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.SaveButton, Language.English), saveButtonTextureEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.MainMenuButton, Language.English), mainMenuButtonEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.GermanButton, Language.English), germanButtonEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.EnglishButton, Language.English), englishButtonEnglish);

        // Other screens - GER
        mTranslatedAssets.Add((TranslatedAssetTypes.AchievementScreen, Language.English), achievementScreenBackgroundEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.StatisticsScreen, Language.English), statisticsScreenBackgroundEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.OptionsScreen, Language.English), optionsScreenBackgroundEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.ControlsScreen, Language.English), controlsScreenBackgroundEnglish);
        mTranslatedAssets.Add((TranslatedAssetTypes.CreditsScreen, Language.English), creditsScreenBackgroundEnglish);
    }

    internal static Rectangle GetRectangleFromId16(int spriteId)
    {
        return new Rectangle(spriteId % 64 * 16, spriteId / 64 * 16, 16, 16);
    }

    public Texture2D GetTranslatedAsset(TranslatedAssetTypes type)
    {
        return mTranslatedAssets[(type, Game1.mLanguage)];
    }
}