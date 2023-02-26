using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revanche.Extensions;
using Revanche.Input;
using System;
using Revanche.Core;

namespace Revanche.Screens.Game;

public class SelectionRenderer
{
    private const int I2 = 2;

    private const float F8 = 8f;

    private bool mIsDragged;
    private Vector2 mStartPoint;
    private Vector2 mEndPoint;
    private readonly MouseListener mMouseListener;
    private readonly Camera mCamera;
    
    public SelectionRenderer(Camera camera)
    {
        mMouseListener = new MouseListener();
        mCamera = camera;
    }

    public void HandleInput()
    {
        mMouseListener.Update();

        if (mMouseListener.WasPressedLmb())
        {
            mStartPoint = mCamera.CameraToWorld(mMouseListener.GetMousePosition());
            mEndPoint = mCamera.CameraToWorld(mMouseListener.GetMousePosition());
        }

        else if (mMouseListener.IsHeldLmb())
        {
            mEndPoint = mCamera.CameraToWorld(mMouseListener.GetMousePosition());
            if (Vector2.Distance(mStartPoint, mEndPoint) > F8)
            {
                mIsDragged = true;
            }
        }

        else if (mMouseListener.WasReleasedLmb())
        {
            mIsDragged = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!mIsDragged)
        {
            return;
        }
        spriteBatch.DrawRectangleOutline(GetSelectionRectangle(), (int)(I2/mCamera.Zoom) + 1, Color.Red);
    }

    public Rectangle GetSelectionRectangle()
    {
        var minX = (int)Math.Min(mStartPoint.X, mEndPoint.X);
        var minY = (int)Math.Min(mStartPoint.Y, mEndPoint.Y);
        var maxX = (int)Math.Max(mStartPoint.X, mEndPoint.X);
        var maxY = (int)Math.Max(mStartPoint.Y, mEndPoint.Y);
        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }
}