using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.Items;

internal sealed class HealthPotion : Item
{
    private const int Sprite = 2952;
    
    public HealthPotion(Vector2 position) : base(position,
        new List<ItemEffects>() { ItemEffects.HealingEffect})
    {
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
    }
}