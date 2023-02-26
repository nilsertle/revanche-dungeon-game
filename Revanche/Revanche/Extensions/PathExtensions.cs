using System.Collections.Generic;
using Revanche.Core;
using Revanche.GameObjects;
using Revanche.Map.Pathfinding;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Revanche.Extensions
{
    internal static class PathExtensions
    {
        private const int SummonLimit = 6;

        internal static void MultiP(LevelState levelState, IPathfinder pathfinder, List<Character> characters, Vector2 pixelGoal)
        {
            var gridGoal = pixelGoal.ToGrid();

            if (characters.Count <= SummonLimit) // possibly remove this if
            {
                foreach (var character in characters)
                {
                    character.SetPath(pathfinder.CalculatePath(character.Position.ToGrid(), gridGoal));
                }
                return;
            }

            var freeTargets = FindNFreeTargets(gridGoal, levelState, characters.Count);
            if (freeTargets.Count <= 0)
            {
                return;
            }
            for (var i = 0; i < characters.Count; i++)
            {
                characters[i].SetPath(pathfinder.CalculatePath(characters[i].Position.ToGrid(), freeTargets[i]));
            }
        }

        private static List<Vector2> FindNFreeTargets(this Vector2 gridGoal, LevelState levelState, int num)
        {
            if (!levelState.GameMap.Passable(gridGoal))
            {
                return new List<Vector2>();
            }

            var points = new List<Vector2> { gridGoal };
            var i = 0;
            while (points.Count < num)
            {
                foreach (var p in levelState.GameMap.PassableNeighbors(points[i]))
                {
                    if (!points.Contains(p))
                    {
                        points.Add(p);
                    }
                }
                i++;
            }

            return points;
        }
    }
}
