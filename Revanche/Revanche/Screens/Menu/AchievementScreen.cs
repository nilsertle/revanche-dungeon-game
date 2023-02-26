using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.AchievementSystem;
using Revanche.Core;
using Revanche.Managers;
using Revanche.Screens.Menu.UIComponents;
using Revanche.Sound;

namespace Revanche.Screens.Menu;

public sealed class AchievementScreen : MenuScreen
{
    private readonly AchievementManager mAchievementManager;
    private readonly AssetManager mAssetManager;

    public AchievementScreen(EventDispatcher eventDispatcher, AssetManager assetManager, AchievementManager achievementManager)
    {
        mEventDispatcher = eventDispatcher;
        mAchievementManager = achievementManager;
        mAssetManager = assetManager;
        mBackgroundTexture = assetManager.GetTranslatedAsset(TranslatedAssetTypes.AchievementScreen);
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        var panePosition = Game1.mCenter;
        var buttonList = new List<MenuElement>();
        var achievementList = mAchievementManager.Achievements;
        var yOffset = new Vector2(0, 100);
        var xOffset = new Vector2(300, 0);
        for (var i = 0; i < achievementList.Count; i++)
        {
            var color = Color.IndianRed;
            if (achievementList[i].IsUnlocked)
            {
                color = Color.Green;
            }
            // ReSharper disable once PossibleLossOfFraction
            // I want the loss of fraction (integer division with rounding) to specify the rows
            var achievementButton = new StringButton(panePosition + (-1 + i%3) * xOffset + (i/3 * yOffset),
                true,
                achievementList[i].Name + "\n " + achievementList[i].Hint + " (" + achievementList[i].Progress + "/" + achievementList[i].UnlockThreshold + ")\n" + " Freigeschaltet: " + achievementList[i].IsUnlocked switch {false => "Nein", true => "Ja"},
                color, mAssetManager.mFont);
            buttonList.Add(achievementButton);
        }

        var exitButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ExitButton), panePosition - yOffset - xOffset, new Vector2(128, 64), true);
        exitButton.Subscribe(OnExitButtonClick);
        buttonList.Add(exitButton);

        var statButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.StatisticsButton),
            panePosition - yOffset + xOffset,
            new Vector2(128, 64),
            true);
        statButton.Subscribe(OnStatButtonClick);
        buttonList.Add(statButton);

        var clearButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.DeleteButton),
            panePosition - yOffset,
            new Vector2(128, 64),
            true);
        clearButton.Subscribe(OnClearButtonPress);
        buttonList.Add(clearButton);

        var pane = new Pane(buttonList,
            Vector2.Zero,
            panePosition);

        mMenuElements.Add(pane);
    }

    private void OnExitButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
    }

    private void OnStatButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.StatisticsMenu());
    }

    private void OnClearButtonPress()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mAchievementManager.ClearAchievements();
        RebuildScreen();
    }
}