using Microsoft.Xna.Framework.Graphics;
using Revanche.Input;
using Revanche.Managers;
using System;
using System.Collections.Generic;
using Revanche.Screens.Game;
using Revanche.Screens.Menu;

namespace Revanche.Screens;

internal sealed class ScreenManager
{
    private readonly List<IScreen> mScreenStack;
    private readonly ScreenFactory mScreenFactory;

    internal ScreenManager(ScreenFactory screenFactory, EventDispatcher eventDispatcher)
    {
        // Events
        eventDispatcher.OnScreenRequest += ScreenRequestHandler;
        eventDispatcher.OnSaveRequest += Save;

        mScreenStack = new List<IScreen>();
        mScreenFactory = screenFactory;
        AddScreen(mScreenFactory.CreateMainMenuScreen());
    }

    /// <summary>
    /// Add a new screen on top of the stack
    /// </summary>
    /// <param name="screen"></param>
    private void AddScreen(IScreen screen)
    {
        mScreenStack.Add(screen);
    }

    /// <summary>
    /// Removes the screen at the top of the stack
    /// </summary>
    private void RemoveScreen()
    {
        if (mScreenStack.Count > 0)
        {
            mScreenStack.RemoveAt(mScreenStack.Count - 1);
        }
    }

    /// <summary>
    /// Passes user-input to the screen
    /// on top of the stack and updates it
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <param name="action"></param>
    internal void Update(float deltaTime, InputState action)
    {
        var screenDepth = mScreenStack.Count;

        if (screenDepth > 0)
        {
            for (var i = 1; i < screenDepth; i++)
            {
                if (mScreenStack[i].UpdateLower)
                {
                    mScreenStack[i - 1].Update(deltaTime);
                }
            }
            mScreenStack[^1].Update(deltaTime);
            mScreenStack[^1].HandleInput(action);
        }
    }

    /// <summary>
    /// Draws the screen on top of the stack
    /// </summary>
    /// <param name="spriteBatch"></param>
    internal void Draw(SpriteBatch spriteBatch)
    {
        var screenDepth = mScreenStack.Count;
        if (screenDepth > 0)
        {
            for (var i = 1; i < screenDepth; i++)
            {
                if (mScreenStack[i].DrawLower)
                {
                    mScreenStack[i - 1].Draw(spriteBatch);
                }
            }

            mScreenStack[screenDepth - 1].Draw(spriteBatch);
        }
    }

    internal void RebuildScreens()
    {
        var screenDepth = mScreenStack.Count;
        if (screenDepth > 0)
        {
            for (int i = screenDepth - 1; i >= 0; i--)
            {
                mScreenStack[i].RebuildScreen();
            }
        }
    }

    private void PopAllScreens() 
    {
        while (mScreenStack.Count > 0)
        {
            mScreenStack.RemoveAt(mScreenStack.Count - 1);
        }
    }

    // Event handling -----------------------------------------------------------------
    private void ScreenRequestHandler(INavigationEvent navigationEvent)
    {
        switch (navigationEvent)
        {
            case INavigationEvent.NewGame e:
                AddScreen(mScreenFactory.CreateGameScreen(e.mLevelState));
                break;
            case INavigationEvent.MainMenu:
                AddScreen(mScreenFactory.CreateMainMenuScreen());
                break;
            case INavigationEvent.OptionMenu e:
                AddScreen(mScreenFactory.CreateOptionMenu(e.mParent));
                break;
            case INavigationEvent.LoadMenu:
                AddScreen(mScreenFactory.CreateLoadGameMenu());
                break;
            case INavigationEvent.PauseMenu e:
                AddScreen(mScreenFactory.CreatePauseMenu(e.CanSave));
                break;
            case INavigationEvent.PopScreen:
                RemoveScreen();
                break;
            case INavigationEvent.PopAll:
                PopAllScreens();
                break;
            case INavigationEvent.ControlsScreen:
                AddScreen(mScreenFactory.CreateControlsScreen());
                break;
            case INavigationEvent.ControlsImageScreen e:
                AddScreen(mScreenFactory.CreateControlsImageScreen(e.BackgroundIndex));
                break;
            case INavigationEvent.GameOverScreen:
                AddScreen(mScreenFactory.CreateGameOverScreen());
                break;
            case INavigationEvent.GameWonScreen:
                AddScreen(mScreenFactory.CreateGameWonScreen());
                break;
            case INavigationEvent.AchievementMenu:
                AddScreen(mScreenFactory.CreateAchievementScreen());
                break;
            case INavigationEvent.StatisticsMenu:
                AddScreen(mScreenFactory.CreateStatisticsScreen());
                break;
            case INavigationEvent.CreditsScreen:
                AddScreen(mScreenFactory.CreateCreditsScreen());
                break;
            case INavigationEvent.TechDemoMenu:
                AddScreen(mScreenFactory.CreateTechDemoMenu());
                break;
            case INavigationEvent.TalentMenu e:
                AddScreen(mScreenFactory.CreateTalentMenu(e.CurrentSkills, e.mSkillPoints, e.mOnSkillPointsUpdate));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(navigationEvent), navigationEvent, null);

        }
    }

    private void Save()
    {
        var pm = (PauseMenu)mScreenStack[^1];
        var gs = (GameScreen)mScreenStack[^2];
        pm.Save(gs.CurrentLevelState);
    }
}