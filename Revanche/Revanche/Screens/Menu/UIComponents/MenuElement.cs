using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revanche.Input;

namespace Revanche.Screens.Menu.UIComponents;

public abstract class MenuElement
{
    public Vector2 Position { get; protected init; }
    protected Vector2 Size { get; init; }
    protected bool Visible { get; set; }
    public abstract bool Interact(InputState inputState);
    public abstract void Draw(SpriteBatch spriteBatch);
}