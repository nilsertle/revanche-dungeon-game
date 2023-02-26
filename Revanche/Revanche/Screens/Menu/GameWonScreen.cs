using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.Core;
using Revanche.Managers;
using Revanche.Screens.Menu.UIComponents;
using Revanche.Sound;
using static Revanche.Game1;

namespace Revanche.Screens.Menu;

public sealed class GameWonScreen : MenuScreen
{
    private readonly AssetManager mAssetManager;
    public GameWonScreen(EventDispatcher eventDispatcher, AssetManager assetManager)
    {
        mAssetManager = assetManager;
        UpdateLower = false;
        DrawLower = true;
        mEventDispatcher = eventDispatcher;
        mBackgroundTexture = assetManager.mGameWonScreenBackground;
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        var panePos = mCenter;

        var backToMenuButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.MainMenuButton),
            new Vector2(mCenter.X, mCenter.Y + 150),
            new Vector2(100, 100),
            true);
        

        backToMenuButton.Subscribe(OnBackToMenuButtonClick);

        var pane = new Pane(new List<MenuElement>() { backToMenuButton },
            Vector2.Zero,
            panePos);

        mMenuElements.Add(pane);
    }


    private void OnBackToMenuButtonClick()
    {
        mEventDispatcher.StopSound();
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        mEventDispatcher.SendScreenRequest(new INavigationEvent.MainMenu());
    }
}