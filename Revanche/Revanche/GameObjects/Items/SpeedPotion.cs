using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.GameObjects.Items;

internal sealed class
    SpeedPotion : Item
{
    private const int Sprite = 2960;

    public SpeedPotion(Vector2 position) : base(
        position,
        new List<ItemEffects>() { ItemEffects.TimedSpeedUpEffect})
    {
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
    }
}