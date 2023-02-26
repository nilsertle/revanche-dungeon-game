using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revanche.Input;
using Revanche.Managers;

namespace Revanche.Screens.Menu.UIComponents;

public sealed class TextLabel : MenuElement
{
    private readonly string mText;
    private readonly Color mTextColor;

    public TextLabel(Vector2 position, string text, Color textColor)
    {
        Position = position;
        Size = AssetManager.mHudFont.MeasureString(text);
        mText = text;
        mTextColor = textColor;
    }

    public override bool Interact(InputState inputState)
    {
        return false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var originFactor = 2f;
        spriteBatch.DrawString(AssetManager.mHudFont, mText, Position, mTextColor, 0f, Size/originFactor, Vector2.One, SpriteEffects.None, 0f);
    }
}