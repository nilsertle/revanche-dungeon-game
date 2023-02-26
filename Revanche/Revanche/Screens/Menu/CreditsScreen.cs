using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.Core;
using Revanche.Managers;
using Revanche.Screens.Menu.UIComponents;
using Revanche.Sound;

namespace Revanche.Screens.Menu;

public sealed class CreditsScreen: MenuScreen
{
    private readonly AssetManager mAssetManager;
        public CreditsScreen(EventDispatcher eventDispatcher, AssetManager assetManager)
        {
            mEventDispatcher = eventDispatcher;
            mAssetManager = assetManager;
            mBackgroundTexture = assetManager.GetTranslatedAsset(TranslatedAssetTypes.CreditsScreen);
            CreateMenuElements();
        }

        protected override void CreateMenuElements()
        {
            var backToMenuButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ReturnButton),
                new Vector2(74,42),
                new Vector2(128, 64),
                true);

            backToMenuButton.Subscribe(OnBackToMenuButtonClick);
            var pane = new Pane(new List<MenuElement>() {backToMenuButton},Vector2.Zero, Game1.mCenter);
            mMenuElements.Add(pane);
        }

        private void OnBackToMenuButtonClick()
        {
            mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
            mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        }
}