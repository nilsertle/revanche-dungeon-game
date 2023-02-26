#nullable enable
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revanche.Input;

namespace Revanche.Screens.Menu.UIComponents;

public sealed class Pane : MenuElement
{
    private const float F2 = 2f;

    private readonly List<MenuElement> mMenuElements;
    private readonly Texture2D? mPaneTexture;

    public Pane(List<MenuElement> menuElements, Vector2 size, Vector2 position, Texture2D? paneTexture = null)
    {

        mMenuElements = menuElements;
        mPaneTexture = paneTexture;
        Size = size;
        Position = position;
    }

    public override bool Interact(InputState inputState)
    {
        foreach (var element in mMenuElements)
        {
            if (element.Interact(inputState))
            {
                return true;
            }
        }

        return false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (mPaneTexture != null)
        {
            spriteBatch.Draw(mPaneTexture, Position, null, Color.White, 0f, new Vector2(mPaneTexture.Width / F2, mPaneTexture.Height / F2), Vector2.One, SpriteEffects.None, 0f);
        }
        foreach (var menuElement in mMenuElements)
        {
            menuElement.Draw(spriteBatch);
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, Rectangle rect)
    {
        spriteBatch.Draw(mPaneTexture,rect, Color.White);
        foreach (var menuElement in mMenuElements)
        {
            menuElement.Draw(spriteBatch);
        }
    }
}