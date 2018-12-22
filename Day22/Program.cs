using System;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;

namespace Day22
{
    enum Tool
    {
        Climbing,
        Torch,
        Neither
    }

    class Program
    {
        //static int depth = 510;
        //static int targetX = 10;
        //static int targetY = 10;

        static int depth = 4080;
        static int targetX = 14;
        static int targetY = 785;

        static int Heuristic((int x, int y, Tool tool) pos)
        {
            return Math.Abs(pos.x - targetX) + Math.Abs(pos.y - targetY)/* + pos.tool != Tool.Torch ? 7 : 0*/;
        }

        static bool IsLegal((int x, int y, Tool tool) pos, int[,] erosion)
        {
            switch (erosion[pos.x, pos.y] % 3)
            {
                case 0:
                    return pos.tool != Tool.Neither;
                case 1:
                    return pos.tool != Tool.Torch;
                case 2:
                    return pos.tool != Tool.Climbing;
            }
            return false; // unreachable
        }

        static IEnumerable<((int x, int y, Tool tool) neighbour, int neighbourDist)> GetNeighbours((int x, int y, Tool tool) current)
        {
            if (current.x > 0)
            {
                yield return ((current.x - 1, current.y, current.tool), 1);
            }
            if (current.y > 0)
            {
                yield return ((current.x, current.y - 1, current.tool), 1);
            }
            yield return ((current.x + 1, current.y, current.tool), 1);
            yield return ((current.x, current.y + 1, current.tool), 1);
            // it doesn't hurt to add an extra 7 minute path to the current state
            yield return ((current.x, current.y, Tool.Climbing), 7);
            yield return ((current.x, current.y, Tool.Torch), 7);
            yield return ((current.x, current.y, Tool.Neither), 7);
            yield break;
        }

        static int Shortest(int[,] erosion, int targetX, int targetY)
        {
            HashSet<(int x, int y, Tool tool)> visited = new HashSet<(int x, int y, Tool tool)>();
            IPriorityQueue<(int x, int y, Tool tool), int> frontier = new SimplePriorityQueue<(int x, int y, Tool tool), int>();

            frontier.Enqueue((0, 0, Tool.Torch), 0);
            // Do we need to keep track of where we came from, perhaps not...
            Dictionary<(int x, int y, Tool tool), int> cost = new Dictionary<(int x, int y, Tool tool), int>();
            cost.Add((0, 0, Tool.Torch), 0);

            Dictionary<(int x, int y, Tool tool), int> remainingCost = new Dictionary<(int x, int y, Tool tool), int>();
            remainingCost.Add((0, 0, Tool.Torch), Heuristic((0, 0, Tool.Torch)));

            Dictionary<(int x, int y, Tool tool), int> costEstimate = new Dictionary<(int x, int y, Tool tool), int>();
            (int x, int y, Tool tool) start = (0, 0, Tool.Torch);
            costEstimate.Add(start, cost[start] + remainingCost[start]);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (current.x == targetX && current.y == targetY && current.tool == Tool.Torch)
                {
                    return costEstimate[current];
                }
                visited.Add(current);
                foreach (((int x, int y, Tool tool) neighbour, int neighbourCost) n in GetNeighbours(current))
                {
                    if (!visited.Contains(n.neighbour) && IsLegal(n.neighbour, erosion))
                    {
                        int tentativeCost = cost[current] + n.neighbourCost;
                        int newEstimate = tentativeCost + Heuristic(n.neighbour);
                        if (!frontier.Contains(n.neighbour))
                        {
                            frontier.Enqueue(n.neighbour, tentativeCost + Heuristic(n.neighbour));
                            cost.Add(n.neighbour, tentativeCost);
                            costEstimate.Add(n.neighbour, newEstimate);
                        } else if (tentativeCost < cost[n.neighbour])
                        {
                            cost[n.neighbour] = tentativeCost;
                            costEstimate[n.neighbour] = newEstimate;
                            frontier.UpdatePriority(n.neighbour, newEstimate);
                        }
                    }
                }
            }
            return -1;
         }

        static void Main(string[] args)
        {
            int MaxY = targetY + 1000;
            int MaxX = targetX + 1000;

            int[,] gi = new int[MaxX, MaxY];
            int[,] erosion = new int[MaxX, MaxY];

            for (int y = 0; y < MaxY; ++y)
            {
                for (int x = 0; x < MaxX; ++x)
                {
                    if (x == 0 && y == 0)
                    {
                        gi[x, y] = 0;
                    } else if (x == targetX && y == targetY)
                    {
                        gi[x, y] = 0;
                    } else if (y == 0)
                    {
                        gi[x, y] = x * 16807;
                    } else if (x == 0)
                    {
                        gi[x, y] = y * 48271;
                    } else
                    {
                        gi[x, y] = erosion[x - 1, y] * erosion[x, y - 1];
                    }
                    erosion[x, y] = (gi[x, y] + depth) % 20183;
                }
            }

            int riskLevel = 0;
            for (int y = 0; y <= targetY; ++y)
            {
                for (int x = 0; x <= targetX; ++x)
                {
                    riskLevel += erosion[x, y] % 3;
                }
            }

            Console.WriteLine($"Risk level = {riskLevel}");

            int dist = Shortest(erosion, targetX, targetY);
        }
    }
}
