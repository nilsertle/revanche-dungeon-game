using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.Core;
using Revanche.Managers;
using Revanche.Screens.Menu.UIComponents;
using Revanche.Sound;

namespace Revanche.Screens.Menu;

internal sealed class MainMenuScreen : MenuScreen
{
    private readonly SaveManager mSaveManager;
    private readonly AssetManager mAssetManager;
    public MainMenuScreen(AssetManager assetManager, EventDispatcher eventDispatcher, SaveManager saveManager)
    {
        mEventDispatcher = eventDispatcher;
        mSaveManager = saveManager;
        mAssetManager = assetManager;
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        mBackgroundTexture = mAssetManager.mMainMenuPaneBackground;
        var panePos = Game1.mCenter - new Vector2(0, 115);
        var offset = new Vector2(0, 66);

        var newGameButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.NewGameButton), panePos, new Vector2(128, 64), true);
        newGameButton.Subscribe(OnNewGameButtonClick);

        var loadButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.LoadButton), panePos + offset ,  new Vector2(128, 64), true);
        loadButton.Subscribe(OnLoadButtonClick);

        var achievementButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.AchievementButton), panePos + 2 * offset, new Vector2(128, 64), true);
        achievementButton.Subscribe(OnAchievementButtonClick);

        var techDemoButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.TrainingButton), panePos + 3 * offset, new Vector2(128, 64), true);
        techDemoButton.Subscribe(OnTechDemoButtonClick);

        var optionsButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.OptionsButton), panePos + 4 * offset, new Vector2(128, 64), true);
        optionsButton.Subscribe(OnOptionsButtonClick);
        
        var controlsButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ControlsButton), panePos + 5 * offset, new Vector2(128, 64), true);
        controlsButton.Subscribe(OnControlsButtonClick);
        
        var creditsButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.CreditsButton), panePos + 6 * offset, new Vector2(128, 64), true);
        creditsButton.Subscribe(OnCreditsButtonClick);

        var exitButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ExitButton), panePos + 7 * offset, new Vector2(128, 64), true);

        exitButton.Subscribe(OnExitButtonClick);


        var pane = new Pane(new List<MenuElement>() { newGameButton, loadButton, achievementButton, techDemoButton, optionsButton, controlsButton, creditsButton, exitButton },
            Vector2.Zero,
            panePos);

        mMenuElements.Add(pane);
    }

    private void OnNewGameButtonClick()
    {
        mEventDispatcher.StopSound();
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendAudioRequest(new SongEvent(Songs.Roaming));
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.GameStart, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        mEventDispatcher.SendScreenRequest(new INavigationEvent.NewGame(LevelState.CreateDefaultLevelState()));
        mSaveManager.ResetCurrentSave();
    }

    private void OnLoadButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.LoadMenu());
    }

    private void OnControlsButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.ControlsScreen());
    }

    private void OnCreditsButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.CreditsScreen());
    }

    private void OnExitButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.CloseGame();
    }

    private void OnTechDemoButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.TechDemoMenu());
    }

    private void OnOptionsButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.OptionMenu(this));
    }

    private void OnAchievementButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.AchievementMenu());
    }
}