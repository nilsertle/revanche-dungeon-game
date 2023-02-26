using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Revanche.Core;

namespace Revanche.Map.Pathfinding
{
    public sealed class AStarPathfinder : IPathfinder
    {
        private IWeightedGraph MapGraph { get; } 

        public AStarPathfinder(LevelState levelState)
        {
            MapGraph = levelState.GameMap;
        }

        public List<Vector2> CalculatePath(Vector2 start, Vector2 goal)
        {
            if (!MapGraph.Passable(goal))
            {
                return new List<Vector2>();
            }

            Dictionary<Vector2, Vector2> cameFrom = new();
            Dictionary<Vector2, double> costSoFar = new();
            var calculatedPath = new List<Vector2>();
            // Set the start and goal locations
            
            // Calculate using A*
            var frontier = new PriorityQueue<Vector2, double>();
            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            costSoFar[start] = 0;


            while (frontier.Count > 0)
            {
                var currentLocation = frontier.Dequeue();

                if (currentLocation.Equals(goal))
                {
                    break;
                }

                foreach (var nextLocation in MapGraph.PassableNeighbors(currentLocation))
                {
                    var newCost = costSoFar[currentLocation] + MapGraph.Cost(currentLocation, nextLocation);
                    if (costSoFar.ContainsKey(nextLocation) && newCost >= costSoFar[nextLocation])
                    {
                        continue;
                    }

                    costSoFar[nextLocation] = newCost;
                    var priority = newCost + Heuristic(nextLocation, goal);
                    frontier.Enqueue(nextLocation, priority);
                    cameFrom[nextLocation] = currentLocation;
                }
            }

            var location = goal;
            while (location != start)
            {
                calculatedPath.Add(location);
                location = cameFrom[location];
            }
            calculatedPath.Add(start);
            return calculatedPath.Select(Camera.TileCenterToWorld).ToList();
        }

        private double Heuristic(Vector2 a, Vector2 b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

    }
}
