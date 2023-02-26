using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Revanche.Map.Pathfinding;

public interface IPathfinder
{
    public List<Vector2> CalculatePath(Vector2 start, Vector2 goal);
}