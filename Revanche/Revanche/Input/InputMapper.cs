using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Revanche.Core;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using static Revanche.Input.BasicActionType;

namespace Revanche.Input;
internal sealed class InputMapper
{
    private const float RectangleThreshold = 8f;

    private readonly KeyboardListener mKeyboardListener;
    private readonly MouseListener mMouseListener;

    // Vectors for drag selection
    private Vector2 mRectangleStart;
    private Vector2 mRectangleEnd;

    private readonly Dictionary<KeyEvent, IActionType> mKeyActionMapping = new ();
    public InputMapper()
    {
        mKeyboardListener = new KeyboardListener ();
        mMouseListener = new MouseListener ();
        // Default key mappings, add your new actions here
        mKeyActionMapping.Add(new KeyEvent(Keys.A), new IActionType.Basic(DamageSpell));
        mKeyActionMapping.Add(new KeyEvent(Keys.Y), new IActionType.Basic(HealSpell));
        mKeyActionMapping.Add(new KeyEvent(Keys.S), new IActionType.Basic(SpeedSpell));
        mKeyActionMapping.Add(new KeyEvent(Keys.F), new IActionType.Basic(Interact));
        mKeyActionMapping.Add(new KeyEvent(Keys.Q), new IActionType.Summon(SummonType.Demon));
        mKeyActionMapping.Add(new KeyEvent(Keys.W), new IActionType.Summon(SummonType.Skeleton));
        mKeyActionMapping.Add(new KeyEvent(Keys.E), new IActionType.Summon(SummonType.StormCloud));
        mKeyActionMapping.Add(new KeyEvent(Keys.R), new IActionType.Summon(SummonType.WaterElemental));
        mKeyActionMapping.Add(new KeyEvent(Keys.T), new IActionType.Summon(SummonType.MagicSeedling));
        mKeyActionMapping.Add(new KeyEvent(Keys.D1), new IActionType.Select(0));
        mKeyActionMapping.Add(new KeyEvent(Keys.D2), new IActionType.Select(1));
        mKeyActionMapping.Add(new KeyEvent(Keys.D3), new IActionType.Select(2));
        mKeyActionMapping.Add(new KeyEvent(Keys.D4), new IActionType.Select(3));
        mKeyActionMapping.Add(new KeyEvent(Keys.D5), new IActionType.Select(4));
        mKeyActionMapping.Add(new KeyEvent(Keys.D6), new IActionType.Select(5));
        mKeyActionMapping.Add(new KeyEvent(Keys.Space), new IActionType.Basic(JumpToPlayer));
        mKeyActionMapping.Add(new KeyEvent(Keys.K), new IActionType.Basic(DebugMode));
        mKeyActionMapping.Add(new KeyEvent(Keys.N), new IActionType.Basic(NextLevel));
        mKeyActionMapping.Add(new KeyEvent(Keys.M), new IActionType.Basic(KillAll));
        mKeyActionMapping.Add(new KeyEvent(Keys.Tab), new IActionType.Basic(SelectAll));
        mKeyActionMapping.Add(new KeyEvent(Keys.Escape), new IActionType.Basic(Escape));
    }

    /// <summary>
    /// Returns a single InputState based on mouse and keyboard events
    /// </summary>
    /// <returns></returns>
    public InputState Update()
    {
        mKeyboardListener.Update();
        mMouseListener.Update();

        var mouseAction = GetMouseAction();
        var keyAction = GetKeyboardAction();

        return new InputState(mMouseListener.GetMousePosition(),
            mouseAction,
            keyAction);
    }

    private IActionType GetKeyboardAction()
    {

        foreach (var keyValuePair in mKeyActionMapping)
        {
            switch (keyValuePair.Key.mType)
            {
                case KeyEventType.OnButtonDown:
                    // Wip
                    break;
                case KeyEventType.OnButtonPressed:
                    if (mKeyboardListener.WasPressed(keyValuePair.Key.mKey))
                    {
                        return keyValuePair.Value;
                    }
                    break;
                case KeyEventType.OnButtonUp:
                    // Wip
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return new IActionType.Basic(None);
    }

    private IActionType GetMouseAction()
    {
        if (mMouseListener.WasPressedLmb())
        {
            mRectangleStart = mMouseListener.GetMousePosition();
            mRectangleEnd = mMouseListener.GetMousePosition();

        }
        else if (mMouseListener.IsHeldLmb())
        {
            mRectangleEnd = mMouseListener.GetMousePosition();
            return new IActionType.Basic(None);
        }
        else if (mMouseListener.WasReleasedLmb())
        {
            if (Vector2.Distance(mRectangleStart, mRectangleEnd) > RectangleThreshold)
            {
                return new IActionType.Basic(DragSelect);
            }
            return new IActionType.Basic(Select);
        }
        
        if (mMouseListener.WasClickedRmb())
        {
            return new IActionType.Basic(Command);
        }

        if (mMouseListener.WasScrolledUp())
        {
            return new IActionType.Basic(ZoomIn);
        }

        if (mMouseListener.WasScrolledDown())
        {
            return new IActionType.Basic(ZoomOut);
        }

        return new IActionType.Basic(None);
    }
}

/// <summary>
/// Action type specifies a given action that was
/// extracted from our keybinds. There are three
/// different sub actions
/// </summary>
public interface IActionType
{
    /// <summary>
    /// Basic actions keep an enum value only. For when
    /// you do not need to carry data
    /// </summary>
    class Basic : IActionType
    {
        public readonly BasicActionType mBasicAction;
        public Basic(BasicActionType basicAction)
        {
            mBasicAction = basicAction;
        }
    }

    /// <summary>
    /// A summon action that contains a summon type to be summoned
    /// </summary>
    class Summon : IActionType
    {
        public readonly SummonType mSummonType;
        public Summon(SummonType summonType)
        {
           mSummonType = summonType;
        }
    }

    /// <summary>
    /// A select action to select units with hotkeys 1-6.
    /// Carries the index of what unit to select
    /// </summary>
    class Select : IActionType
    {
        public readonly int mSelectedIndex;

        public Select(int selectedIndex)
        {
            mSelectedIndex = selectedIndex;
        }
    }
}

public enum BasicActionType
{
    Select,
    DragSelect,
    Command,
    DamageSpell,
    HealSpell,
    SpeedSpell,
    Interact,
    JumpToPlayer,
    DebugMode,
    NextLevel,
    ZoomIn,
    ZoomOut,
    KillAll,
    SelectAll,
    Escape,
    None
}

internal sealed class KeyEvent
{
    public readonly Keys mKey;
    public readonly KeyEventType mType;

    public KeyEvent(Keys key, KeyEventType type = KeyEventType.OnButtonPressed)
    {
        mKey = key;
        mType = type;
    }
}

internal enum KeyEventType
{
    OnButtonDown,
    OnButtonPressed,
    OnButtonUp
}

public sealed class InputState
{
    internal Vector2 MousePosition { get; }
    internal IActionType KeyAction { get; }
    internal IActionType MouseAction { get; }

    internal InputState(Vector2 mousePosition, IActionType mouseAction, IActionType keyAction)
    {
        MousePosition = mousePosition;
        KeyAction = keyAction;
        MouseAction = mouseAction;
    }
}