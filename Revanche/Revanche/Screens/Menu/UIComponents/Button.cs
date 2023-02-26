#nullable enable
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revanche.Input;
using Revanche.Managers;

namespace Revanche.Screens.Menu.UIComponents;

public sealed class Button : MenuElement
{
    private const float F05 = 0.7f;
    private const float F2 = 2f;

    private const int I2 = 2;

    public event Action? OnClick;
    private readonly Texture2D mButtonTexture;
    private bool mIsHovered;
    private string? mText;
    private readonly string? mToolTipText;
    private readonly SpriteFont? mFont;
    private readonly int mIconSpriteId = -1;
    private Rectangle mButtonRectangle;
    private readonly Vector2 mIconPosition = Vector2.Zero;
    private float mRotation;

    // Standard button
    internal Button(Texture2D buttonTexture, Vector2 position, Vector2 size, bool visible)
    {
        mButtonTexture = buttonTexture;
        Position = position;
        Size = size;
        Visible = visible;
        mIsHovered = false;
        mButtonRectangle = new Rectangle((int)(Position.X - Size.X/F2), (int)(Position.Y - Size.Y/F2), (int)Size.X, (int)Size.Y);
    }

    // Button with text
    internal Button(Texture2D buttonTexture, Vector2 position, Vector2 size, bool visible, string text, SpriteFont font)
    {
        mButtonTexture = buttonTexture;
        Position = position;
        Size = size;
        Visible = visible;
        mIsHovered = false;
        mText = text;
        mFont = font;
        mButtonRectangle = new Rectangle((int)(Position.X - Size.X / F2), (int)(Position.Y - Size.Y / F2), (int)Size.X, (int)Size.Y);
    }

    // Button with text and icon
    internal Button(Texture2D buttonTexture, Vector2 position, Vector2 size, bool visible, string text, SpriteFont font, int iconSpriteId, Vector2 iconPosition)
    {
        mButtonTexture = buttonTexture;
        Position = position;
        Size = size;
        Visible = visible;
        mIsHovered = false;
        mText = text;
        mFont = font;
        mButtonRectangle = new Rectangle((int)(Position.X - Size.X / F2), (int)(Position.Y - Size.Y / F2), (int)Size.X, (int)Size.Y);
        mIconSpriteId = iconSpriteId;
        mIconPosition = iconPosition;
    }

    internal Button(Texture2D buttonTexture, Vector2 position, Vector2 size, bool visible, string text, SpriteFont font, int iconSpriteId, Vector2 iconPosition, string toolTipText)
    {
        mButtonTexture = buttonTexture;
        Position = position;
        Size = size;
        Visible = visible;
        mIsHovered = false;
        mText = text;
        mToolTipText = toolTipText;
        mFont = font;
        mButtonRectangle = new Rectangle((int)(Position.X - Size.X / F2), (int)(Position.Y - Size.Y / F2), (int)Size.X, (int)Size.Y);
        mIconSpriteId = iconSpriteId;
        mIconPosition = iconPosition;
    }


    public override bool Interact(InputState inputState)
    {
        if (!Visible)
        {
            return false;
        }

        if (mButtonRectangle.Contains(inputState.MousePosition))
        {
            mIsHovered = true;
            if (inputState.MouseAction is IActionType.Basic { mBasicAction: BasicActionType.Select })
            {
                OnClick?.Invoke();
                return true;
            }
        }
        else
        {
            mIsHovered = false;
        }

        return false;
    }

    internal void SetText(string text)
    {
        mText = text;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Visible)
        {
            spriteBatch.Draw(mButtonTexture,
                Position,
                null,
                GetColor(),
                0f,
                new Vector2(mButtonTexture.Width / F2, mButtonTexture.Height / F2),
                Vector2.One,
                SpriteEffects.None,
                0f);
            if (mIconSpriteId != -1)
            {
                spriteBatch.Draw(AssetManager.mSpriteSheet, mIconPosition, AssetManager.GetRectangleFromId16(mIconSpriteId), GetColor(), 0f, Game1.mOrigin, Game1.mScale/I2, SpriteEffects.None, 0f );
            }
            if (mText != null)
            {
                spriteBatch.DrawString(mFont, mText, Position, Color.Black, mRotation, mFont!.MeasureString(mText)/F2, Vector2.One, SpriteEffects.None, 0f);
            }

            if (mToolTipText != null && mIsHovered)
            {
                spriteBatch.DrawString(mFont, mToolTipText, new Vector2(Game1.mScreenWidth/2f-100, mFont!.MeasureString(mText).Y*1.5f), Color.White, 0f, mFont!.MeasureString(mText) / F2, Vector2.One, SpriteEffects.None, 0f);
            }
        }
    }

    public void RotateText(float rotationAngle)
    {
        mRotation = rotationAngle;
    }

    private Color GetColor()
    {
        return mIsHovered ? Color.White * F05 : Color.White;
    }

    internal void Subscribe(Action buttonAction)
    {
        OnClick += buttonAction;
    }
}