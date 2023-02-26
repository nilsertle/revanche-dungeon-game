using Microsoft.Xna.Framework;

namespace Revanche.GameObjects.Environment;

public class BloodShrine : GameObject
{
    private const float BloodShrineLayer = 0.8f;

    public BloodShrine(Vector2 position) : base(position)
    {
        LayerDepth = BloodShrineLayer;
    }
}