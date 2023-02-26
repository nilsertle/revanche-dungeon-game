using Microsoft.Xna.Framework.Graphics;
using Revanche.Input;

namespace Revanche.Screens;

internal interface IScreen
{
    bool UpdateLower { get; }
    bool DrawLower { get; } 
    void Update(float deltaTime);
    void Draw(SpriteBatch spriteBatch);
    void HandleInput(InputState inputState);
    void RebuildScreen();
}