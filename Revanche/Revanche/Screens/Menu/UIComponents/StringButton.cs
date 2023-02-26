#nullable enable
using Microsoft.Xna.Framework.Graphics;
using Revanche.Extensions;
using Revanche.Input;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Revanche.Screens.Menu.UIComponents;

/// <summary>
/// String buttons will have a rectangular outline and their size and hitbox adjust to the
/// offered string (experimental class)
/// </summary>
internal sealed class StringButton : MenuElement
{
    private const float F2 = 2f;
    private const float HoverFilter = 0.5f;
    private const int LineWidth = 2;

    private bool mIsHovered;
    private readonly string mText;
    private readonly string? mAlternateText;
    private readonly SpriteFont mFont;
    private Rectangle mButtonRectangle;
    private readonly Color mColor;

    internal StringButton(Vector2 position, bool visible, string text, Color color, SpriteFont font, string? alternateText=null)
    {
        Position = position;
        Visible = visible;
        mIsHovered = false;
        mText = " " + text + " ";
        mAlternateText = alternateText;
        mFont = font;
        Size = mFont.MeasureString(mText);
        mButtonRectangle = new Rectangle((int)(Position.X - Size.X / F2), (int)(Position.Y - Size.Y / F2), (int)Size.X, (int)Size.Y);
        mColor = color;
    }

    public override bool Interact(InputState inputState)
    {
        if (mButtonRectangle.Contains(inputState.MousePosition))
        {
            mIsHovered = true;
            if (inputState.MouseAction is IActionType.Basic { mBasicAction: BasicActionType.Select })
            {
                return true;
            }
        }
        else
        {
            mIsHovered = false;
        }

        return false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (mIsHovered && mAlternateText != null)
        {
            Visible = false;
            spriteBatch.DrawString(mFont,
                mAlternateText,
                new Vector2(mButtonRectangle.X, mButtonRectangle.Y),
                GetColor(mColor));
            return;
        }

        Visible = true;

        if (!Visible)
        {
            return;
        }
        spriteBatch.DrawRectangleOutline(mButtonRectangle, LineWidth, GetColor(mColor));
        spriteBatch.DrawString(mFont,
        mText,
        Position,
        GetColor(mColor),
        0f, Size / F2,
        Vector2.One,
        SpriteEffects.None,
        0f);
    }

    private Color GetColor(Color color)
    {
        return mIsHovered ? color * HoverFilter : color;
    }

}