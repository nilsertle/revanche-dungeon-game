using Microsoft.Xna.Framework;

namespace Revanche.GameObjects.Environment
{
    internal class DestructableWall : WallObject
    {
        public DestructableWall(Vector2 position, int hits) : base(position, hits)
        {
            HitsLeft = hits;
        }
    }
}
