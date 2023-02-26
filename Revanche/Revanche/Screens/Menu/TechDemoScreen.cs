using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.Core;
using Revanche.Managers;
using Revanche.Screens.Menu.UIComponents;
using Revanche.Sound;
using static Revanche.Core.EnemyType;
using static Revanche.Game1;

namespace Revanche.Screens.Menu;

public sealed class TechDemoScreen : MenuScreen
{
    private const int I5 = 5;

    private readonly AssetManager mAssetManager;

    private readonly List<EnemyType> mEnemyTypes = new()
        { ConanTheBarbarian, FrontPensioner, Paladin, BombMagician, Pirate, ArchEnemy};
    public TechDemoScreen(AssetManager assetManager, EventDispatcher eventDispatcher)
    {
        mAssetManager = assetManager;
        mEventDispatcher = eventDispatcher;
        mBackgroundTexture = mAssetManager.mTechDemoBackground;
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        var center = mCenter - new Vector2(mScreenWidth*0.4f, mScreenHeight*0.4f);
        var xOffset = new Vector2(mScreenWidth/5f, 0);
        var yOffset = new Vector2(0, mScreenHeight/6f);
        for (var i = 0; i < I5; i++)
        {
            for (var j = 0; j < I5; j++)
            {
                var buttonPos = center + j * xOffset + i * yOffset;
                var button = new Button(mAssetManager.mBaseButtonTexture,
                    buttonPos,
                    new Vector2(128, 64),
                    true,
                    "",
                    mAssetManager.mFont, (int)mEnemyTypes[i] + (j * 64), buttonPos);
                var action = OpenAiDemo(i, j+1);
                button.Subscribe(action);
                mMenuElements.Add(button);
            }
        }

        var archEnemyButtonPos = center + 5 * yOffset;
        var archEnemyButton = new Button(mAssetManager.mBaseButtonTexture,
            archEnemyButtonPos,
            new Vector2(128, 64),
            true,
            "",
            mAssetManager.mFont, (int)mEnemyTypes[5], archEnemyButtonPos);
        archEnemyButton.Subscribe(() => {mEventDispatcher.SendScreenRequest(new INavigationEvent.NewGame(LevelState.CreateAiTestLevelState(mEnemyTypes[I5], 1))); });
        mMenuElements.Add(archEnemyButton);

        var stressTestButtonPos = center + 5 * yOffset + xOffset;
        var stressTestButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.DemoButton),
            stressTestButtonPos,
            new Vector2(128, 64),
            true);
        stressTestButton.Subscribe(() => {
            mEventDispatcher.SendScreenRequest(new INavigationEvent.PopAll());
            mEventDispatcher.SendScreenRequest(new INavigationEvent.NewGame(LevelState.CreateTechDemoLevelState()));});
        mMenuElements.Add(stressTestButton);

        var exitButtonPos = center + 5 * yOffset + 2 * xOffset;
        var exitButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ReturnButton),
            exitButtonPos,
            new Vector2(128, 64),
            true);
        exitButton.Subscribe(() =>
        {
            mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
            mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        });
        mMenuElements.Add(exitButton);
    }

    private Action OpenAiDemo(int index, int level)
    {
        return () =>
        {
            mEventDispatcher.SendScreenRequest(new INavigationEvent.PopAll());
            mEventDispatcher.SendScreenRequest(
                new INavigationEvent.NewGame(LevelState.CreateAiTestLevelState(mEnemyTypes[index], level)));
        };
    }
}