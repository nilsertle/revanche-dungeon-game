using Microsoft.Xna.Framework;

namespace Revanche.Extensions;

public static class Vector2Extensions
{
    public static Vector2 ToGrid(this Vector2 vec)
    {
        return new Vector2((int)(vec.X / Game1.sScaledPixelSize), (int)(vec.Y / Game1.sScaledPixelSize));
    }
}