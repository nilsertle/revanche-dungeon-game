using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.Core;
using Revanche.Managers;
using Revanche.Screens.Menu.UIComponents;
using Revanche.Sound;
using static Revanche.Game1;

namespace Revanche.Screens.Menu;

public sealed class OptionMenu : MenuScreen
{
    private const int BigResolutionX = 1400;
    private const int BigResolutionY = 1000;
    private const int SmallResolutionX = 1200;
    private const int SmallResolutionY = 800;

    private readonly AssetManager mAssetManager;
    private readonly MenuScreen mParent;
    public OptionMenu(EventDispatcher eventDispatcher, AssetManager assetManager, MenuScreen parent)
    {
        UpdateLower = false;
        DrawLower = true;
        mEventDispatcher = eventDispatcher;
        mAssetManager = assetManager;
        mBackgroundTexture = assetManager.GetTranslatedAsset(TranslatedAssetTypes.OptionsScreen);
        mParent = parent;
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        var panePos = mCenter;
        var soundVolume = mEventDispatcher.SoundVolumeRequest;
        var musicVolume = mEventDispatcher.MusicVolumeRequest;

        var increaseSoundButton = new Button(mAssetManager.mDefaultButtonTexture,
            new Vector2(mCenter.X + mScreenWidth * 0.2f, mCenter.Y + 10),
            new Vector2(273, 50),
            true,
            "+",
            AssetManager.mHudFont);

        var decreaseSoundButton = new Button(mAssetManager.mDefaultButtonTexture,
            new Vector2(mCenter.X + mScreenWidth * 0.2f, mCenter.Y + 60),
            new Vector2(273, 50),
            true,
            "-",
            AssetManager.mHudFont);

        var soundBar1 = new Bar(mAssetManager.mSoundButtonTexture, new Rectangle((int)increaseSoundButton.Position.X-85, (int)(increaseSoundButton.Position.Y-64), 100, 10));
        var soundBar2 = new Bar(mAssetManager.mSoundButtonTexture2, new Rectangle((int)increaseSoundButton.Position.X-85, (int)(increaseSoundButton.Position.Y-64), (int)(soundVolume * 100), 10));

        var increaseMusicButton = new Button(mAssetManager.mDefaultButtonTexture,
            new Vector2(mCenter.X + mScreenWidth * 0.2f, mCenter.Y + mScreenHeight * 0.35f),
            new Vector2(273, 50),
            true,
            "+",
            AssetManager.mHudFont);

        var decreaseMusicButton = new Button(mAssetManager.mDefaultButtonTexture,
            new Vector2(mCenter.X + mScreenWidth * 0.2f, mCenter.Y + mScreenHeight * 0.35f + 50),
            new Vector2(273, 50),
            true,
            "-",
            AssetManager.mHudFont);

        var musicBar1 = new Bar(mAssetManager.mSoundButtonTexture, new Rectangle((int)increaseMusicButton.Position.X - 85, (int)(increaseMusicButton.Position.Y - 64), 100, 10));
        var musicBar2 = new Bar(mAssetManager.mSoundButtonTexture2, new Rectangle((int)increaseMusicButton.Position.X - 85, (int)(increaseMusicButton.Position.Y - 64), (int)(musicVolume * 100), 10));

        var backToMenuButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ReturnButton),
            new Vector2(74, 42),
            new Vector2(128, 64),
            true);

        var fullScreenButton = new Button(mAssetManager.mDefaultButtonTexture,
            new Vector2(mCenter.X - mScreenWidth * 0.3f, mCenter.Y - mScreenHeight * 0.15f),
            new Vector2(273, 50),
            true,
            "Vollbildmodus",
            AssetManager.mHudFont);

        var mediumScreenButton = new Button(mAssetManager.mDefaultButtonTexture,
            new Vector2(mCenter.X - mScreenWidth * 0.3f, mCenter.Y - mScreenHeight * 0.15f + 60),
            new Vector2(273, 50),
            true,
            "1400 * 1000",
            AssetManager.mHudFont);

        var smallScreenButton = new Button(mAssetManager.mDefaultButtonTexture,
            new Vector2(mCenter.X - mScreenWidth * 0.3f, mCenter.Y - mScreenHeight * 0.15f + 120),
            new Vector2(273, 50),
            true,
            "1200 * 800",
            AssetManager.mHudFont);

        var germanButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.GermanButton),
            new Vector2(mCenter.X - mScreenWidth * 0.38f, mCenter.Y + mScreenHeight * 0.35f),
            new Vector2(128, 64), true);

        var englishButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.EnglishButton),
            new Vector2(mCenter.X - mScreenWidth * 0.22f, mCenter.Y + mScreenHeight * 0.35f),
            new Vector2(128, 64), true);

        backToMenuButton.Subscribe(OnBackToMenuButtonClick);
        fullScreenButton.Subscribe(OnFullScreenButtonClick);
        mediumScreenButton.Subscribe(OnMediumScreenButtonClick);
        smallScreenButton.Subscribe(OnSmallScreenButtonClick);
        increaseSoundButton.Subscribe(OnIncreaseSoundButtonClick);
        decreaseSoundButton.Subscribe(OnDecreaseSoundButtonClick);
        increaseMusicButton.Subscribe(OnIncreaseMusicButtonClick);
        decreaseMusicButton.Subscribe(OnDecreaseMusicButtonClick);
        germanButton.Subscribe(OnGermanButtonClick);
        englishButton.Subscribe(OnEnglishButtonClick);

        var pane = new Pane(
            new List<MenuElement> { 
                fullScreenButton,
                mediumScreenButton,
                smallScreenButton,
                backToMenuButton,
                increaseSoundButton,
                decreaseSoundButton,
                soundBar1,
                soundBar2,
                musicBar1,
                musicBar2,
                decreaseMusicButton,
                increaseMusicButton,
                germanButton,
                englishButton
            },
            Vector2.Zero,
            panePos);

        mMenuElements.Add(pane);
        mMenuElements.Add(soundBar1);
        mMenuElements.Add(soundBar2);
    }

    private void OnBackToMenuButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
    }

    private void OnFullScreenButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.BarClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        mEventDispatcher.SendFullScreenRequest();
    }
    
    private void OnMediumScreenButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.BarClick, null));
        mFullScreen = false;
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        mEventDispatcher.SendResolutionRequest(new ResolutionEvent(BigResolutionX, BigResolutionY));
    }

    private void OnSmallScreenButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.BarClick, null));
        mFullScreen = false;
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        mEventDispatcher.SendResolutionRequest(new ResolutionEvent(SmallResolutionX, SmallResolutionY));
    }

    private void OnGermanButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        if (mLanguage == Language.German) { return; }
        mLanguage = Language.German;
        RebuildScreen();
        mParent.RebuildScreen();
        mBackgroundTexture = mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.OptionsScreen);
    }
    private void OnEnglishButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        if (mLanguage == Language.English) {return;}
        mLanguage = Language.English;
        RebuildScreen();
        mParent.RebuildScreen();
        mBackgroundTexture = mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.OptionsScreen);
    }

    private void OnIncreaseSoundButtonClick()
    {
        var soundVolume = Math.Min(mEventDispatcher.SoundVolumeRequest + 0.2f, 1);
        mEventDispatcher.SendAudioRequest(new VolumeEvent(soundVolume, VolumeMode.ChangeSoundVolume));
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.BarClick, null));
        RebuildScreen();
    }

    private void OnDecreaseSoundButtonClick()
    {
        var soundVolume = Math.Max(mEventDispatcher.SoundVolumeRequest - 0.2f, 0);
        mEventDispatcher.SendAudioRequest(new VolumeEvent(soundVolume, VolumeMode.ChangeSoundVolume));
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.BarClick, null));
        RebuildScreen();
    }

    private void OnIncreaseMusicButtonClick()
    {
        var musicVolume = Math.Min(mEventDispatcher.MusicVolumeRequest + 0.2f, 1);
        mEventDispatcher.SendAudioRequest(new VolumeEvent(musicVolume, VolumeMode.ChangeMusicVolume));
        mEventDispatcher.SendAudioRequest(new SongEvent(Songs.BarClick, false));
        RebuildScreen();
    }

    private void OnDecreaseMusicButtonClick()
    {
        var musicVolume = Math.Max(mEventDispatcher.MusicVolumeRequest - 0.2f, 0);
        mEventDispatcher.SendAudioRequest(new VolumeEvent(musicVolume, VolumeMode.ChangeMusicVolume));
        mEventDispatcher.SendAudioRequest(new SongEvent(Songs.BarClick, false));
        RebuildScreen();
    }
}