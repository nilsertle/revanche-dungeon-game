using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Revanche.GameObjects.Environment
{
    internal class WallObject : GameObject
    {
        [JsonProperty] public int HitsLeft { get; set; }
        public WallObject(Vector2 position, int hits) : base(position)
        {
            HitsLeft = hits;
        }
    }
}
