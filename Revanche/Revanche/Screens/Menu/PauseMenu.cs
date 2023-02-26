using Revanche.Managers;
using Revanche.Screens.Menu.UIComponents;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.Core;
using Revanche.Input;
using Revanche.Sound;

namespace Revanche.Screens.Menu;

internal sealed class PauseMenu : MenuScreen
{
    private readonly SaveManager mSaveManager;
    private readonly AssetManager mAssetManager;
    private readonly bool mCanSave;
    public PauseMenu(AssetManager assetManager, EventDispatcher eventDispatcher, SaveManager saveManager, bool canSave)
    {
        DrawLower = true;
        mEventDispatcher = eventDispatcher;
        mSaveManager = saveManager;
        mAssetManager = assetManager;
        mCanSave = canSave;
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        var panePos = Game1.mCenter;
        var offset = new Vector2(0, 100);
        var standardButtonSize = new Vector2(128, 64);

        var continueButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ContinueButton), panePos - 2 * offset, standardButtonSize, true);
        continueButton.Subscribe(OnContinueButtonClick);

        var saveButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.SaveButton), panePos - offset, standardButtonSize, mCanSave);
        saveButton.Subscribe(OnSaveButtonClick);

        var optionButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.OptionsButton), panePos, standardButtonSize, true);
        optionButton.Subscribe(OnOptionButtonClick);
        
        var controlsButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ControlsButton), panePos + offset, new Vector2(128, 64), true);
        controlsButton.Subscribe(OnControlsButtonClick);

        var exitButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ExitButton), panePos + 2* offset, standardButtonSize, true);
        exitButton.Subscribe(OnExitButtonClick);

        var pane = new Pane(new List<MenuElement>() { continueButton, saveButton, optionButton, controlsButton, exitButton },
            Vector2.Zero,
            panePos,
            mAssetManager.mPausePaneBackground);
        mMenuElements.Add(pane);
        mEventDispatcher.PauseSound();
    }

    private void OnContinueButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.ResumeSound();
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
    }

    private void OnControlsButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.ControlsScreen());
    }
    private void OnOptionButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.OptionMenu(this));
    }

    private void OnExitButtonClick()
    {
        mEventDispatcher.StopSound();
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopAll());
        mEventDispatcher.SendScreenRequest(new INavigationEvent.MainMenu());
    }

    private void OnSaveButtonClick()
    {
        mEventDispatcher.SendPopupEvent(new IPopupEvent.SavePopUp());
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendSaveRequest();
    }

    public void Save(LevelState levelState)
    {
        mSaveManager.SaveGame(levelState);
    }

    public override void HandleInput(InputState inputState)
    {
        if (inputState.KeyAction is IActionType.Basic { mBasicAction: BasicActionType.Escape })
        {
            mEventDispatcher.ResumeSound();
            mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
            return;
        }
        base.HandleInput(inputState);
    }
}