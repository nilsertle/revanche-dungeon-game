using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revanche.Core;
using Revanche.Managers;
using Revanche.Screens.Menu.UIComponents;
using Revanche.Sound;

namespace Revanche.Screens.Menu;

public sealed class ControlsImageScreen : MenuScreen
{
    private int mBackgroundIndex;
    private readonly List<Texture2D> mBackgroundList;
    private readonly AssetManager mAssetManager;
    public ControlsImageScreen(EventDispatcher eventDispatcher, AssetManager assetManager, int backgroundIndex)
    {
        mEventDispatcher = eventDispatcher;
        mBackgroundIndex = backgroundIndex;
        mAssetManager = assetManager;
        mBackgroundList = new List<Texture2D>()
        {
            mAssetManager.mSelectionBackground,
            mAssetManager.mSelectionBackground2,
            mAssetManager.mCameraBackground, 
            mAssetManager.mMovementBackground,
            mAssetManager.mPauseBackground,
            mAssetManager.mInteractionBackground, 
            mAssetManager.mAttackBackground,
            mAssetManager.mSummoningBackground,
            mAssetManager.mMagicBackground,
            mAssetManager.mSkillsMenuBackground
        };
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        mBackgroundTexture = mBackgroundList[mBackgroundIndex];
        var backToMenuButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ReturnButton),
            new Vector2(74, 42),
            new Vector2(128, 64),
            true);
        var prevButton = new Button(mAssetManager.mBaseButtonTexture,
            new Vector2(Game1.mCenter.X - 3 * Game1.mCenter.X / 5, 42),
            new Vector2(128, 64),
            true, "<", mAssetManager.mFont);
        var nextButton = new Button(mAssetManager.mBaseButtonTexture,
            new Vector2(Game1.mCenter.X + 3 * Game1.mCenter.X / 5, 42),
            new Vector2(128, 64),
            true, " >", mAssetManager.mFont);

        prevButton.Subscribe(OnPrevButtonClick);
        nextButton.Subscribe(OnNextButtonClick);
        backToMenuButton.Subscribe(OnBackToMenuButtonClick);
        var pane = new Pane(new List<MenuElement>() { backToMenuButton, prevButton, nextButton }, Vector2.Zero, Game1.mCenter);
        mMenuElements.Add(pane);
    }

    private void OnNextButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mBackgroundIndex = (mBackgroundIndex + 1) % mBackgroundList.Count;
        mBackgroundTexture = mBackgroundList[mBackgroundIndex];
    }

    private void OnPrevButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mBackgroundIndex = (mBackgroundIndex - 1) % mBackgroundList.Count;
        if (mBackgroundIndex < 0)
        {
            mBackgroundIndex += mBackgroundList.Count;
        }
        mBackgroundTexture = mBackgroundList[mBackgroundIndex];
    }

    private void OnBackToMenuButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
    }
}