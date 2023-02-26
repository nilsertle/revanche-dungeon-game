using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revanche.Input;
using Revanche.Managers;
using Revanche.Screens.Menu.UIComponents;

namespace Revanche.Screens.Menu;

public abstract class MenuScreen : IScreen
{
    protected readonly List<MenuElement> mMenuElements = new();
    protected EventDispatcher mEventDispatcher;
    protected Texture2D mBackgroundTexture = null;
    protected bool mScaleBackground = true;
    public bool UpdateLower { get; protected init; } // = false by default
    public bool DrawLower { get; protected init; } // = false by default 

    public virtual void Update(float deltaTime)
    {
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
        if (mBackgroundTexture != null && mScaleBackground)
        {
            spriteBatch.Draw(mBackgroundTexture, new Rectangle(0, 0, Game1.mScreenWidth, Game1.mScreenHeight), Color.White);
        } else if (mBackgroundTexture != null)
        {
            spriteBatch.Draw(mBackgroundTexture, Game1.mCenter,null, Color.White, 0f, new Vector2(mBackgroundTexture.Width/2f, mBackgroundTexture.Height/2f), Vector2.One, SpriteEffects.None, 0f);
        }

        foreach (var menuElement in mMenuElements)
        {
            menuElement.Draw(spriteBatch);
        }
        spriteBatch.End();
    }

    public virtual void HandleInput(InputState inputState)
    {
        foreach (var menuElement in mMenuElements)
        {
            if (menuElement.Interact(inputState))
            {
                return;
            }
        }
    }

    protected abstract void CreateMenuElements();

    public void RebuildScreen()
    {
        mMenuElements.Clear();
        CreateMenuElements();
    }
}