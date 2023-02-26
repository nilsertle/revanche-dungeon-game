using Revanche.GameObjects;

namespace Revanche.Extensions;

public static class GameObjectExtensions
{
    public static bool IntersectsWith(this GameObject gameObject1, GameObject gameObject2)
    {
        return gameObject1.Hitbox.Intersects(gameObject2.Hitbox);
    }
}