using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.Items
{
    internal class DamagePotion : Item
    {
        private const int Sprite = 2968;

        public DamagePotion(Vector2 position) : base(position,
            new List<ItemEffects>() { ItemEffects.TimedDamageUpEffect })
        {
            SpriteId = Sprite;
            CurrentSpriteId = SpriteId;
        }
    }
}

