using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.Core;
using Revanche.Managers;
using Revanche.Screens.Menu.UIComponents;
using Revanche.Sound;

namespace Revanche.Screens.Menu;

public sealed class ControlsScreen : MenuScreen
{
    private readonly AssetManager mAssetManager;
    public ControlsScreen(EventDispatcher eventDispatcher, AssetManager assetManager)
    {
        mEventDispatcher = eventDispatcher;
        mAssetManager = assetManager;
        mBackgroundTexture = assetManager.GetTranslatedAsset(TranslatedAssetTypes.ControlsScreen);
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        var panePos = Game1.mCenter;
        var offset = -150;
        var gap = 75;
        var gapBetween = 130;
        var backToMenuButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ReturnButton),
            new Vector2(74, 42),
            new Vector2(128, 64), true);

        // Left margin
        var selectionButton1 = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.Selection1Button), new Vector2(panePos.X - gapBetween, panePos.Y + offset),
            new Vector2(128, 64), true);

        var cameraButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.CameraButton), new Vector2(panePos.X - gapBetween, panePos.Y + offset + gap * 1),
            new Vector2(128, 64), true);

        var pauseButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.PauseButton), new Vector2(panePos.X - gapBetween, panePos.Y + offset + gap * 2),
            new Vector2(128, 64), true);

        var attackButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.AttackButton), new Vector2(panePos.X - gapBetween, panePos.Y + offset + gap * 3),
            new Vector2(128, 64), true);

        var magicButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.SpellButton), new Vector2(panePos.X - gapBetween, panePos.Y + offset + gap * 4),
            new Vector2(128, 64), true);


        // Right margin
        var selectionButton2 = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.Selection2Button), new Vector2(panePos.X + gapBetween, panePos.Y + offset),
            new Vector2(128, 64), true);

        var movementButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.MovementButton), new Vector2(panePos.X + gapBetween, panePos.Y + offset + gap * 1),
            new Vector2(128, 64), true);

        var interactionButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.InteractionButton), new Vector2(panePos.X + gapBetween, panePos.Y + offset + gap * 2),
            new Vector2(128, 64), true);

        var summoningButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.SummoningButton), new Vector2(panePos.X + gapBetween, panePos.Y + offset + gap * 3),
            new Vector2(128, 64), true);

        var skillsButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.TalentsButton), new Vector2(panePos.X + gapBetween, panePos.Y + offset + gap * 4),
            new Vector2(128, 64), true);

        backToMenuButton.Subscribe(OnBackToMenuButtonClick);
        attackButton.Subscribe(OnAttackButtonClick);
        cameraButton.Subscribe(OnCameraButtonClick);
        interactionButton.Subscribe(OnInteractionButtonClick);
        magicButton.Subscribe(OnMagicButtonClick);
        movementButton.Subscribe(OnMovementButtonClick);
        pauseButton.Subscribe(OnPauseButtonClick);
        selectionButton1.Subscribe(OnSelection1ButtonClick);
        selectionButton2.Subscribe(OnSelection2ButtonClick);
        skillsButton.Subscribe(OnSkillsButtonClick);
        summoningButton.Subscribe(OnSummoningButtonClick);

        var pane = new Pane(new List<MenuElement>() { backToMenuButton, summoningButton, attackButton, cameraButton, interactionButton, magicButton, movementButton, pauseButton, selectionButton1, selectionButton2, skillsButton, summoningButton},
            Vector2.Zero, 
            panePos);

        mMenuElements.Add(pane);
    }

    private void OnSelection1ButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.ControlsImageScreen(0));
    }
    private void OnSelection2ButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.ControlsImageScreen(1));
    }
    private void OnCameraButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.ControlsImageScreen(2));
    }
    private void OnMovementButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.ControlsImageScreen(3));
    }
    private void OnPauseButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.ControlsImageScreen(4));
    }
    private void OnInteractionButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.ControlsImageScreen(5));
    }
    private void OnAttackButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.ControlsImageScreen(6));
    }
    private void OnSummoningButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.ControlsImageScreen(7));
    }
    private void OnMagicButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.ControlsImageScreen(8));
    }
    private void OnSkillsButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.ControlsImageScreen(9));
    }
    private void OnBackToMenuButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
    }
}