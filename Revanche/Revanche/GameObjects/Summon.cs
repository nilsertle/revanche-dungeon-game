using Microsoft.Xna.Framework;

namespace Revanche.GameObjects;

public abstract class Summon: Character
{
    protected Summon(Vector2 position, int level, bool friendly) : base(position, level, friendly)
    {
    }
}