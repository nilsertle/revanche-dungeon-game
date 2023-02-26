using Microsoft.Xna.Framework.Graphics;
using Revanche.Core;
using Revanche.Managers;
using Revanche.Input;

namespace Revanche.Screens.Game;

internal sealed class GameScreen : IScreen
{
    public LevelState CurrentLevelState { get; }
    private readonly HudManager mHudManager;
    private readonly GameInputHandler mInputHandler;
    private readonly GameLogic mGameLogic;
    private readonly SelectionRenderer mSelectionRenderer;
    private readonly TargetRenderer mTargetRenderer;
    public bool UpdateLower => false;
    public bool DrawLower => false;


    internal GameScreen(LevelState levelState, EventDispatcher eventDispatcher, AssetManager assetManager)
    {
        CurrentLevelState = levelState;
        CurrentLevelState.EventDispatcher = eventDispatcher;
        mHudManager = new HudManager(CurrentLevelState, assetManager);
        mGameLogic = new GameLogic(levelState, eventDispatcher);
        mSelectionRenderer = new SelectionRenderer(levelState.Camera2d);
        mInputHandler = new GameInputHandler(CurrentLevelState, eventDispatcher, mGameLogic, mSelectionRenderer);
        mTargetRenderer = new TargetRenderer();
    }

    public void Update(float deltaTime)
    {
        CurrentLevelState.UpdateGameObjects(deltaTime);
        CurrentLevelState.UpdateQuadTree();
        CurrentLevelState.UpdateFogOfWar();
        mGameLogic.Update(deltaTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, transformMatrix: CurrentLevelState.Camera2d.Transform);
        CurrentLevelState.Draw(spriteBatch);
        mSelectionRenderer.Draw(spriteBatch);
        mTargetRenderer.Draw(spriteBatch, CurrentLevelState);
        spriteBatch.End();

        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
        mHudManager.Draw(spriteBatch);
        spriteBatch.End();
    }

    public void HandleInput(InputState inputState)
    {
        mSelectionRenderer.HandleInput();
        mInputHandler.HandleInput(inputState);
    }

    public void RebuildScreen()
    {
       CurrentLevelState.Camera2d.UpdateCamera();
    }
}