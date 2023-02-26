using Microsoft.Xna.Framework.Input;

namespace Revanche.Input;

public class KeyboardListener
{
    // For keyboard button inputs
    private KeyboardState mOldKeyState;
    private KeyboardState mCurrentKeyState;
    public KeyboardListener()
    {
        mOldKeyState = Keyboard.GetState();
        mCurrentKeyState = Keyboard.GetState();
    }

    public void Update()
    {
        mOldKeyState = mCurrentKeyState;
        mCurrentKeyState = Keyboard.GetState();
    }

    public bool WasPressed(Keys key)
    {
        return !mOldKeyState.IsKeyDown(key) && mCurrentKeyState.IsKeyDown(key);
    }
}