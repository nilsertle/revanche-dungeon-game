using System;
using System.Collections.Generic;
using Revanche.Managers;
using Revanche.AchievementSystem;
using Revanche.Core;
using Revanche.GameObjects;
using Revanche.Screens.Game;
using Revanche.Screens.Menu;
using Revanche.Stats;

namespace Revanche.Screens;

internal sealed class ScreenFactory
{
    private readonly AssetManager mAssetManager;
    private readonly EventDispatcher mEventDispatcher;
    private readonly SaveManager mSaveManager;
    private readonly AchievementManager mAchievementManager;
    private readonly StatisticManager mStatManager;

    public ScreenFactory(AssetManager assetManager, EventDispatcher eventDispatcher, SaveManager saveManager, AchievementManager achievementManager, StatisticManager statManager)
    {
        mAssetManager = assetManager;
        mEventDispatcher = eventDispatcher;
        mSaveManager = saveManager;
        mAchievementManager = achievementManager;
        mStatManager = statManager;
    }

    internal IScreen CreateMainMenuScreen()
    {
        return new MainMenuScreen(mAssetManager, mEventDispatcher, mSaveManager);
    }

    internal IScreen CreatePauseMenu(bool canSave)
    {
        return new PauseMenu(mAssetManager, mEventDispatcher, mSaveManager, canSave);
    }

    internal IScreen CreateGameScreen(LevelState levelState)
    {
        return new GameScreen(levelState, mEventDispatcher, mAssetManager);
    }
    
    internal IScreen CreateControlsScreen()
    {
        return new ControlsScreen(mEventDispatcher, mAssetManager);
    }

    internal IScreen CreateControlsImageScreen(int backgroundIndex)
    {
        return new ControlsImageScreen(mEventDispatcher, mAssetManager, backgroundIndex);
    }

    internal IScreen CreateLoadGameMenu()
    {
        return new LoadGameMenu(mEventDispatcher, mAssetManager, mSaveManager);
    }
    
    internal IScreen CreateGameOverScreen()
    {
        return new GameOverScreen(mEventDispatcher, mAssetManager);
    }

    internal IScreen CreateAchievementScreen()
    {
        return new AchievementScreen(mEventDispatcher, mAssetManager, mAchievementManager);
    }

    internal IScreen CreateStatisticsScreen()
    {
        return new StatisticsScreen(mStatManager, mEventDispatcher, mAssetManager);
    }

    internal IScreen CreateGameWonScreen()
    {
        return new GameWonScreen(mEventDispatcher, mAssetManager);
    }

    internal IScreen CreateCreditsScreen()
    {
        return new CreditsScreen(mEventDispatcher, mAssetManager);
    }
    
    internal IScreen CreateOptionMenu(MenuScreen parent)
    {
        return new OptionMenu(mEventDispatcher, mAssetManager, parent);
    }

    internal IScreen CreateTalentMenu(Dictionary<ElementType, int> currentSkills, int skillPoints, Action<TalentTreeResult> onSkillPointsUpdate)
    {
        return new TalentScreen(mAssetManager, mEventDispatcher, currentSkills, skillPoints, onSkillPointsUpdate);
    }

    internal IScreen CreateTechDemoMenu()
    {
        return new TechDemoScreen(mAssetManager, mEventDispatcher);
    }
}