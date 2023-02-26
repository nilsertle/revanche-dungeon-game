using Microsoft.Xna.Framework;
using Revanche.Core;
using System.Collections.Generic;

namespace Revanche.GameObjects.Environment;

internal class TreasureChest : GameObject
{
    private const int I2 = 2;
    private const int I5 = 5;
    private const int Sprite = 3008;

    private const float ChestLayer = 0.8f;

    public readonly List<(DropAbleLoot type, int chance)> mPossibleLoot = new() { (DropAbleLoot.Soul, I5), (DropAbleLoot.HealthPotion, I2) };
    public TreasureChest(Vector2 position) : base(position)
    {
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
        mIsNotAnimated = true;
        LayerDepth = ChestLayer;
    }
}