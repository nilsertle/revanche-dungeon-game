using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Revanche.Input;

public class MouseListener
{
    // For mouse button inputs
    private MouseState mOldMouseState;
    private MouseState mCurrentMouseState;
    public MouseListener()
    {
        mOldMouseState = Mouse.GetState();
        mCurrentMouseState = Mouse.GetState();
    }

    public void Update()
    {
        mOldMouseState = mCurrentMouseState;
        mCurrentMouseState = Mouse.GetState();
    }

    public Vector2 GetMousePosition()
    {
        return mCurrentMouseState.Position.ToVector2();
    }
    public bool WasPressedLmb()
    {
        return (mCurrentMouseState.LeftButton == ButtonState.Pressed &&
                mOldMouseState.LeftButton == ButtonState.Released);
    }
    public bool WasReleasedLmb()
    {
        return (mOldMouseState.LeftButton == ButtonState.Pressed &&
                mCurrentMouseState.LeftButton == ButtonState.Released);
    }

    public bool IsHeldLmb()
    {
        return (mOldMouseState.LeftButton == ButtonState.Pressed &&
                mCurrentMouseState.LeftButton == ButtonState.Pressed);
    }

    public bool WasClickedRmb()
    {
        return (mCurrentMouseState.RightButton == ButtonState.Released &&
                mOldMouseState.RightButton == ButtonState.Pressed);
    }

    public bool WasScrolledUp()
    {
        return mOldMouseState.ScrollWheelValue < mCurrentMouseState.ScrollWheelValue;
    }

    public bool WasScrolledDown()
    {
        return mOldMouseState.ScrollWheelValue > mCurrentMouseState.ScrollWheelValue;
    }
}