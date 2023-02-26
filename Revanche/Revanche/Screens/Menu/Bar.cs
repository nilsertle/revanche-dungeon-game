using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revanche.Input;
using Revanche.Screens.Menu.UIComponents;

namespace Revanche.Screens.Menu;

public class Bar:MenuElement
{
    private readonly Texture2D mBarTexture;
    private readonly Rectangle mBarRectangle;

    public Bar(Texture2D barTexture, Rectangle barRectangle)
    {
        mBarTexture = barTexture;
        mBarRectangle = barRectangle;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(mBarTexture, mBarRectangle, Color.White);
    }

    public override bool Interact(InputState inputState)
    {
        return false;
    }
}