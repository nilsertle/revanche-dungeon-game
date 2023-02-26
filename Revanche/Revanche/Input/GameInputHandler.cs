using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Revanche.AchievementSystem;
using Revanche.Core;
using Revanche.Extensions;
using Revanche.GameObjects;
using Revanche.GameObjects.FriendlyUnits;
using Revanche.Managers;
using Revanche.Screens.Game;
using Revanche.Sound;
using Revanche.Stats;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Revanche.Input;

internal sealed class GameInputHandler
{
    private const int FloorLimit = 5;
    private const float ZoomDelta = 0.1f;

    private readonly EventDispatcher mEventDispatcher;
    private readonly LevelState mLevelState;
    private readonly GameLogic mGameLogic;
    private readonly SelectionRenderer mSelectionRenderer;
    public GameInputHandler(LevelState levelState, EventDispatcher eventDispatcher, GameLogic gameLogic, SelectionRenderer selectionRenderer)
    {
        mLevelState = levelState;
        mGameLogic = gameLogic;
        mEventDispatcher = eventDispatcher;
        mSelectionRenderer = selectionRenderer;
    }

    /// <summary>
    /// Handles IActionType and its subtypes by calling the fitting function
    /// </summary>
    /// <param name="inputState"></param>
    public void HandleInput(InputState inputState)
    {
        // Mouse input
        switch (inputState.MouseAction)
        {
            case IActionType.Basic basic:
                HandleBasicAction(basic, inputState);
                break;
        }

        // Keyboard input
        switch (inputState.KeyAction)
        {
            case IActionType.Basic basic:
                HandleBasicAction(basic, inputState);
                break;
            case IActionType.Summon summon:
                HandleToggleSummonModeAction(summon.mSummonType);
                break;
            case IActionType.Select select:
                HandleToggleUnitAction(select);
                break;
        }
    }

    /// <summary>
    /// Takes the IActionType.Basic type and processes the contained enum value
    /// </summary>
    /// <param name="basic"></param>
    /// <param name="inputState"></param>
    private void HandleBasicAction(IActionType.Basic basic, InputState inputState)
    {
        switch (basic.mBasicAction)
        {
            case BasicActionType.Command:
                var selection = mLevelState.FriendlySummons.Values.Where(c => c.Selected).Cast<Character>().ToList();
                if (mLevelState.Summoner.Selected)
                {
                    selection.Add(mLevelState.Summoner);
                }
                HandleRightClickAction(selection, inputState.MousePosition);
                break;
            case BasicActionType.Select:
                HandleLeftClickAction(inputState);
                break;
            case BasicActionType.DragSelect:
                HandleSelectionRectangle();
                break;
            case BasicActionType.DamageSpell:
                HandleDamageSpellAction(inputState.MousePosition);
                break;
            case BasicActionType.HealSpell:
                HandleHealSpellAction(inputState.MousePosition);
                break;
            case BasicActionType.SpeedSpell:
                HandleSpeedSpellAction(inputState.MousePosition);
                break;
            case BasicActionType.Interact:
                HandleInteractAction();
                break;
            case BasicActionType.JumpToPlayer:
                HandleJumpToPlayerAction();
                break;
            case BasicActionType.SelectAll:
                HandleSelectAllAction();
                break;
            case BasicActionType.Escape:
                HandleEscapeAction();
                break;
            case BasicActionType.ZoomIn:
                mLevelState.Camera2d.AdjustZoom(ZoomDelta);
                break;
            case BasicActionType.ZoomOut:
                mLevelState.Camera2d.AdjustZoom(-ZoomDelta);
                break;
            case BasicActionType.DebugMode:
                Game1.mDebugMode = !Game1.mDebugMode;
                break;
            case BasicActionType.NextLevel:
                if (Game1.mDebugMode && mLevelState.LevelCount < FloorLimit && !mLevelState.InTechDemo)
                {
                    mLevelState.LevelCount++;
                    mLevelState.ChangeLevel();
                }
                break;
            case BasicActionType.KillAll:
                if (!Game1.mDebugMode)
                {
                    return;
                }

                foreach (var hostileSummon in mLevelState.HostileSummons)
                {
                    hostileSummon.Value.CurrentLifePoints = 0;
                }

                break;
        }
    }

    private void HandleRightClickAction(List<Character> characters, Vector2 mousePosition)
    {
        if (characters.Count == 0)
        {
            return;
        }

        var soundEvent = new SoundEvent(SoundEffects.Movement, null);
        if (!mLevelState.GameMap.Passable(mLevelState.Camera2d.CameraToWorld(mousePosition).ToGrid()))
        {
            soundEvent.Sound = SoundEffects.InvalidAction;
        }
        mEventDispatcher.SendAudioRequest(soundEvent);
        // Attack
        var target = mGameLogic.FindTarget(mLevelState.Camera2d.CameraToWorld(mousePosition));
        if (target is { IsFriendly: false })
        {
            foreach (var character in characters)
            {
                character.CurrentState = CharacterState.Attacking;
                mGameLogic.AttackCharacter(character, target);
                character.Selected = false;
            }
            return;
        }
        // Command
        foreach (var character in characters)
        {
            character.CurrentState = CharacterState.PlayerControl;
        }
        mGameLogic.MoveCharacters(characters, mLevelState.Camera2d.CameraToWorld(mousePosition));
    }
    private void HandleLeftClickAction(InputState inputState)
    {
        if (mLevelState.Summoner.SelectedSummonType == null)
        {
            if (mLevelState.QuadTree.PointSearchCharacters(mLevelState.Camera2d.CameraToWorld(inputState.MousePosition))
                    .Where(character => character.IsFriendly).ToList().Count == 0)
            {
                DeselectFriendlies();
                return;
            }

            mLevelState.ActionForFriendlyCharacters((c) => HandleSelectAction(c, inputState.MousePosition));
            return;
        }

        if (mLevelState.IsSummonLimitReached())
        {
            return;
        }

        mGameLogic.SummonFriendlyMonster(mLevelState.Summoner.SelectedSummonType, mLevelState.Camera2d.CameraToWorld(inputState.MousePosition));
    }

    private void HandlePauseGame()
    {
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PauseMenu(!mLevelState.InTechDemo));
    }

    private void HandleDamageSpellAction(Vector2 mousePosition)
    {
        if (mLevelState.Summoner.Selected)
        {
            mGameLogic.ShootFireball(mLevelState.Camera2d.CameraToWorld(mousePosition));
        }
    }

    private void HandleHealSpellAction(Vector2 mousePosition)
    {
        if (mLevelState.Summoner.Selected)
        {
            mGameLogic.ShootHealSpell(mLevelState.Camera2d.CameraToWorld(mousePosition));
        }
    }

    private void HandleSpeedSpellAction(Vector2 mousePosition)
    {
        if (mLevelState.Summoner.Selected)
        {
            mGameLogic.ShootSpeedSpell(mLevelState.Camera2d.CameraToWorld(mousePosition));
        }
    }

    private void HandleInteractAction()
    {
        // Consume item above all else
        mGameLogic.ConsumeItem(mLevelState.Summoner);

        // Ladder interaction
        if (mLevelState.Ladder?.IntersectsWith(mLevelState.Summoner) == true && !mLevelState.InTechDemo)
        {
            mLevelState.ChangeLevel();
            mEventDispatcher.SendAchievementEvent(AchievementType.DungeonExplorer);
            mEventDispatcher.SendStatisticEvent(StatisticType.ExploredFloors);
            return;
        }

        // BloodShrine interaction
        if (!mLevelState.Summoner.IntersectsWith(mLevelState.BloodShrine))
        {
            return;
        }
        mLevelState.Summoner.StopMovement();

        void OnSkillPointsUpdate(TalentTreeResult results)
        {
            mLevelState.Summoner.UpdateSkills(results.NewSkills, results.NewSkillPoints);
            mLevelState.Camera2d.IsLocked = false;
        }

        mLevelState.Camera2d.IsLocked = true;
        mEventDispatcher.SendScreenRequest(new INavigationEvent.TalentMenu(mLevelState.Summoner.Skills, mLevelState.Summoner.SkillPoints, OnSkillPointsUpdate));
        
    }

    private void HandleJumpToPlayerAction()
    {
        mLevelState.Camera2d.SetCameraPosition(mLevelState.Summoner.Position);
    }

    private void HandleToggleSummonModeAction(SummonType? summonType)
    {

        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
        {
            SelectBySummonType(summonType);
            return;
        }

        if (mLevelState.Summoner.SelectedSummonType == summonType || !mLevelState.Summoner.Selected)
        {
            mLevelState.Summoner.SelectedSummonType = null;
            return;
        }
        mLevelState.Summoner.SelectedSummonType = summonType;
    }

    private void HandleSelectAction(Character character, Vector2 mousePosition)
    {
        if (character.Hitbox.Contains(
                mLevelState.Camera2d.CameraToWorld(mousePosition)))
        {
            character.Selected = !character.Selected;
        }
    }

    private void HandleToggleUnitAction(IActionType.Select selectAction)
    {
        if (mLevelState.Summoner.SelectedSummonType != null)
        {
            return;
        }

        var index = selectAction.mSelectedIndex;
        if (index == 0)
        {
            mGameLogic.SelectCharacter(mLevelState.Summoner);
            return;
        }

        if (mLevelState.FriendlySummons.Count <= index - 1)
        {
            return;
        }
        mGameLogic.SelectCharacter(mLevelState.FriendlySummons.Values.ToList()[index - 1]);
    }

    private void HandleSelectAllAction()
    {
        var selectionCount = mLevelState.FriendlySummons.Count(kvp => !kvp.Value.Selected);
        if (!mLevelState.Summoner.Selected || selectionCount > 0)
        {
            mLevelState.Summoner.Selected = true;
            foreach (var kvp in mLevelState.FriendlySummons)
            {
                kvp.Value.Selected = true;
            }
        }
        else
        {
            mLevelState.Summoner.Selected = !mLevelState.Summoner.Selected;
            foreach (var kvp in mLevelState.FriendlySummons)
            {
                kvp.Value.Selected = !kvp.Value.Selected;
            }
        }
    }

    private void HandleEscapeAction()
    {
        if (mLevelState.Summoner.SelectedSummonType != null)
        {
            HandleToggleSummonModeAction(mLevelState.Summoner.SelectedSummonType);
        }
        else
        {
            HandlePauseGame();
        }
    }

    private void HandleSelectionRectangle()
    {
        var gameObjects = mLevelState.QuadTree.Search(mSelectionRenderer.GetSelectionRectangle());
        
        // Deselect everything but the things contained in the rectangle
        DeselectFriendlies();

        // Select everything within the rectangle that was drawn
        foreach (var obj in gameObjects)
        {
            if (obj is Character { IsFriendly: true } character)
            {
                character.Selected = true;
            }
        }
    }
    private void DeselectFriendlies()
    {
        mLevelState.Summoner.Selected = false;
        mLevelState.Summoner.SelectedSummonType = null;
        foreach (var friendly in mLevelState.FriendlySummons)
        {
            friendly.Value.Selected = false;
        }
    }

    private void SelectBySummonType(SummonType? summonType)
    {
        if (summonType == null)
        {
            return;
        }

        switch (summonType)
        {
            case SummonType.Demon:
                SelectBySummonTypeHelper(mLevelState.FriendlySummons.Where(kvp => kvp.Value is Demon).Select(kvp => kvp.Value).ToList());
                break;
            case SummonType.Skeleton:
                SelectBySummonTypeHelper(mLevelState.FriendlySummons.Where(kvp => kvp.Value is Skeleton).Select(kvp => kvp.Value).ToList());
                break;
            case SummonType.StormCloud:
                SelectBySummonTypeHelper(mLevelState.FriendlySummons.Where(kvp => kvp.Value is StormCloud).Select(kvp => kvp.Value).ToList());
                break;
            case SummonType.WaterElemental:
                SelectBySummonTypeHelper(mLevelState.FriendlySummons.Where(kvp => kvp.Value is WaterElemental).Select(kvp => kvp.Value).ToList());
                break;
            case SummonType.MagicSeedling:
                SelectBySummonTypeHelper(mLevelState.FriendlySummons.Where(kvp => kvp.Value is MagicSeedling).Select(kvp => kvp.Value).ToList());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(summonType), summonType, null);
        }
    }

    private void SelectBySummonTypeHelper(List<Summon> summonList)
    {
        if (summonList.Count == 0)
        {
            return;
        }

        DeselectFriendlies();
        foreach (var summon in summonList)
        {
            summon.Selected = true;
        }
    }
}