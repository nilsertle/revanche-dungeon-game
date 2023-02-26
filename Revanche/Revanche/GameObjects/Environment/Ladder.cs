using Microsoft.Xna.Framework;

namespace Revanche.GameObjects.Environment
{
    public sealed class Ladder : GameObject
    {
        // ReSharper disable once MemberCanBeInternal
        // If ladder constructor is made internal,
        // loading a game where the ladder is present
        // will lead to a crash. Always make saveable
        // constructors public please

        private const int Sprite = 3072;
        private const float LadderLayer = 0.8f;

        public Ladder(Vector2 position) : base(position)
        {
            SpriteId = Sprite;
            CurrentSpriteId = SpriteId;
            LayerDepth = LadderLayer;
        }
    }
}
