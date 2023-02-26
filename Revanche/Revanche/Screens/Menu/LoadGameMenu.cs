using System;
using Revanche.Managers;
using Revanche.Screens.Menu.UIComponents;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Revanche.Core;
using Revanche.Sound;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Revanche.Screens.Menu;

public sealed class LoadGameMenu : MenuScreen
{
    private readonly SaveManager mSaveManager;
    private readonly AssetManager mAssetManager;
    private List<string> mSaveFiles;
    private int mPageCount;
    public LoadGameMenu(EventDispatcher eventDispatcher, AssetManager assetManager, SaveManager saveManager)
    {
        DrawLower = true;
        mPageCount = 0;
        mEventDispatcher = eventDispatcher;
        mSaveManager = saveManager;
        mAssetManager = assetManager;
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        mSaveFiles = Directory.GetFiles("Saves").OrderBy(p => new FileInfo(p).CreationTime).ToList();
        mBackgroundTexture = mAssetManager.mMainMenuPaneBackground;
        mMenuElements.Clear();
        var panePos = Game1.mCenter;

        var buttonList = CreateButtonList(panePos);

        var exitButton = new Button(mAssetManager.GetTranslatedAsset(TranslatedAssetTypes.ReturnButton),
            new Vector2(panePos.X, panePos.Y + mAssetManager.mLoadPaneBackgroundTexture.Height/2f - 64),
            new Vector2(128, 64),
            true);
        var upArrowButton = new Button(mAssetManager.mTinyButtonTexture,
            exitButton.Position + new Vector2(32, -64),
            new Vector2(32, 32),
            true, "^", mAssetManager.mFont);
        upArrowButton.Subscribe(OnUpArrowButtonClick);
        var downArrowButton = new Button(mAssetManager.mTinyButtonTexture,
            exitButton.Position + new Vector2(-32, -64),
            new Vector2(32, 32),
            true, "^", mAssetManager.mFont);
        downArrowButton.RotateText(MathHelper.Pi);
        downArrowButton.Subscribe(OnDownArrowButtonClick);
        exitButton.Subscribe(OnReturnButtonClick);
        buttonList.Add(exitButton);
        buttonList.Add(upArrowButton);
        buttonList.Add(downArrowButton);

        var pane = new Pane(buttonList, new Vector2(150, 150), panePos, mAssetManager.mLoadPaneBackgroundTexture);
        mMenuElements.Add(pane);
    }

    private List<MenuElement> CreateButtonList(Vector2 panepos)
    {
        var buttonList = new List<MenuElement>();
        var spacing = mAssetManager.mLoadPaneBackgroundTexture.Height/7f;

        for (var i = mPageCount * 5; i < Math.Min(mSaveFiles.Count, mPageCount * 5 + 5); i++)
        {
            //Saves/Save6.json
            var yOffset = new Vector2(0, (i - (mPageCount * 5)) * spacing);
            var xOffset = new Vector2(64, 0);
            var buttonPos = new Vector2(panepos.X, panepos.Y - mAssetManager.mLoadPaneBackgroundTexture.Height / 2f + 64);
            var loadGameButton = new Button(mAssetManager.mBaseButtonTexture,
                buttonPos + yOffset - xOffset / 2f,
                new Vector2(128, 64),
                true,
                mSaveFiles[i].Substring(6, mSaveFiles[i].IndexOf(".", StringComparison.Ordinal) - 6),
                mAssetManager.mFont);
            var deleteSaveGameButton = new Button(mAssetManager.mSmallButtonTexture,
                buttonPos + yOffset + xOffset,
                new Vector2(64, 64),
                true,
                "X",
                mAssetManager.mFont);
            var loadAction = OnLoadButtonClick(mSaveFiles[i]);
            var deleteAction = OnDeleteButtonClick(mSaveFiles[i]);
            loadGameButton.Subscribe(loadAction);
            deleteSaveGameButton.Subscribe(deleteAction);
            buttonList.Add(loadGameButton);
            buttonList.Add(deleteSaveGameButton);
        }
        return buttonList;
    }

    private Action OnLoadButtonClick(string file)
    {
        return () =>
        {
            var levelState = mSaveManager.LoadGame(file);
            levelState.RefreshQuadTree();
            mEventDispatcher.SendScreenRequest(new INavigationEvent.PopAll());
            mEventDispatcher.SendScreenRequest(new INavigationEvent.NewGame(levelState));
            mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.GameStart, null));
            mEventDispatcher.SendAudioRequest(new SongEvent(Songs.Roaming));
        };
    }

    private Action OnDeleteButtonClick(string file)
    {
        return () =>
        {
            mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
            mSaveManager.DeleteSave(file);
            RebuildScreen();
        };
    }

    private void OnReturnButtonClick()
    {
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
    }

    private void OnUpArrowButtonClick()
    {
        var ceil = mSaveFiles.Count == 0 ? 0 : (int)Math.Ceiling(1.0f*mSaveFiles.Count/5)-1;
        if (mPageCount + 1 <= ceil)
        {
            mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.BarClick, null));
            mPageCount += 1;
            RebuildScreen();
            return;
        }
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.InvalidAction, null));
        mPageCount = ceil;
        RebuildScreen();
    }

    private void OnDownArrowButtonClick()
    {
        if (mPageCount - 1 >= 0)
        {
            mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.BarClick, null));
            mPageCount -= 1;
            RebuildScreen();
            return;
        }
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.InvalidAction, null));
        mPageCount = 0;
        RebuildScreen();
    }
}