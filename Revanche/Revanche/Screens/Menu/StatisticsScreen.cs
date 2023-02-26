using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.Core;
using Revanche.Managers;
using Revanche.Screens.Menu.UIComponents;
using Revanche.Sound;
using Revanche.Stats;

namespace Revanche.Screens.Menu;

public sealed class StatisticsScreen : MenuScreen
{
    private readonly StatisticManager mStatisticManager;
    private readonly AssetManager mAssetManager;
    public StatisticsScreen(StatisticManager statManager, EventDispatcher eventDispatcher, AssetManager assetManager)
    {
        mStatisticManager = statManager;
        mEventDispatcher = eventDispatcher;
        mAssetManager = assetManager;
        mBackgroundTexture = mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.StatisticsScreen);
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        var panePosition = Game1.mCenter;
        var testcolor = new Color(120, 89, 105);
        var stats = mStatisticManager.Stats;
        var yOffset = new Vector2(0, 50);
        var label1 = new TextLabel(panePosition - 3 * yOffset,
            "Gesammelte Punkte: " + stats[StatisticType.AccumulatedPoints], testcolor);
        var label2 = new TextLabel(panePosition - 2 * yOffset,
            "Besiegte Gegner: " + stats[StatisticType.DefeatedEnemies], testcolor);
        var label3 = new TextLabel(panePosition - yOffset,
            "Besiegte Bosse: " + stats[StatisticType.DefeatedBosses], testcolor);
        var label4 = new TextLabel(panePosition,
            "Erkundete Ebenen: " + stats[StatisticType.ExploredFloors], testcolor);
        var label5 = new TextLabel(panePosition + yOffset,
            "Beschworene Monster: " + stats[StatisticType.SummonedMonsters], testcolor);
        var label6 = new TextLabel(panePosition + 2 * yOffset,
            "Gesammelte Erfahrungspunkte: " + stats[StatisticType.GainedExp], testcolor);
        var exitButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ReturnButton), panePosition + 3 *  yOffset + new Vector2(0, 20), new Vector2(128, 64), true);
        exitButton.Subscribe(OnExitButtonClick);
        var labelList = new List<MenuElement>() { label1, label2, label3, label4, label5, label6, exitButton};
        var pane = new Pane(labelList, Vector2.Zero, panePosition);
        mMenuElements.Add(pane);
    }

    private void OnExitButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
    }
}