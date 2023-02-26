using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.Items;

internal sealed class Soul : Item
{
    private const int Sprite = 2944;

    public Soul(Vector2 position) : base(position, 
        new List<ItemEffects>() { ItemEffects.HealingEffect})
    {
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
    }
}