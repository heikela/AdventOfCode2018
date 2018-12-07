using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day6
{
    class Program
    {
        static int Dist(IEnumerable<int> xy, int x, int y)
        {
            int cx = xy.First();
            int cy = xy.Skip(1).First();
            return Math.Abs(cx - x) + Math.Abs(cy - y);
        }

        static void Main(string[] args)
        {
            var lines = File.ReadLines("../../../input.txt");
            var coords = lines.Select(l => l.Split(',').Select(int.Parse));
            var minX = coords.Select(c => c.First()).Min() - 1;
            var maxX = coords.Select(c => c.First()).Max() + 2;
            var minY = coords.Select(c => c.Skip(1).First()).Min() - 1;
            var maxY = coords.Select(c => c.Skip(1).First()).Max() + 2;
            int x, y;

            /*
            var closest = new Dictionary<(int, int), int>();
            for (x = minX; x < maxX; ++x)
            {
                for (y = minY; y < maxY; ++y)
                {
                    int i = 0;
                    int minDist = int.MaxValue;
                    int minId = -1;
                    foreach (var xy in coords)
                    {
                        int dist = Dist(xy, x, y);
                        if (dist == minDist)
                        {
                            minId = -1;
                        }
                        if (dist < minDist)
                        {
                            minId = i;
                            minDist = dist;
                        }
                        ++i;
                    }
                    closest.Add((x, y), minId);
                }
            }
            var potentialWinners = new HashSet<int>();
            {
                int i = 0;
                foreach (var _ in coords)
                {
                    potentialWinners.Add(i);
                    ++i;
                }
            }
            x = minX;
            for (y = minY; y < maxY; ++y)
            {
                int infiniteId = closest[(x, y)];
                if (infiniteId != -1)
                {
                    if (potentialWinners.Contains(infiniteId))
                    {
                        potentialWinners.Remove(infiniteId);
                    }
                }
            }
            x = maxX - 1;
            for (y = minY; y < maxY; ++y)
            {
                int infiniteId = closest[(x, y)];
                if (infiniteId != -1)
                {
                    if (potentialWinners.Contains(infiniteId))
                    {
                        potentialWinners.Remove(infiniteId);
                    }
                }
            }
            y = minY;
            for (x = minX; x < maxX; ++x)
            {
                int infiniteId = closest[(x, y)];
                if (infiniteId != -1)
                {
                    if (potentialWinners.Contains(infiniteId))
                    {
                        potentialWinners.Remove(infiniteId);
                    }
                }
            }
            y = maxY - 1;
            for (x = minX; x < maxX; ++x)
            {
                int infiniteId = closest[(x, y)];
                if (infiniteId != -1)
                {
                    if (potentialWinners.Contains(infiniteId))
                    {
                        potentialWinners.Remove(infiniteId);
                    }
                }
            }

            var winCounts = new Dictionary<int, int>();
            for (x = minX; x < maxX; ++x)
            {
                for (y = minY; y < maxY; ++y)
                {
                    int closestToXY = closest[(x, y)];
                    if (closestToXY != -1 && potentialWinners.Contains(closestToXY))
                    {
                        if (!winCounts.ContainsKey(closestToXY))
                        {
                            winCounts.Add(closestToXY, 1);
                        }
                        else
                        {
                            winCounts[closestToXY]++;
                        }
                    }
                }
            }

            int maxClosest = 0;
            int maxClosestId = -1;

            foreach (var item in winCounts)
            {
                if (item.Value > maxClosest)
                {
                    maxClosest = item.Value;
                    maxClosestId = item.Key;
                }
            }

            Console.WriteLine($"Winner is {maxClosest}");

            */
            maxX = maxX + 50;
            minX = minX - 50;
            maxY += 50;
            minY -= 50;
            int safeRegionSize = 0;
            Console.WriteLine($"The last point is = {coords.Last().First()},{coords.Last().Skip(1).First()}");
            for (x = minX; x < maxX; ++x)
            {
                for (y = minY; y < maxY; ++y)
                {
                    int total = coords.Select(c => Dist(c, x, y)).Sum();
                    if (total < 10000)
                    {
                       // Console.WriteLine($"For point {x},{y} the total distance is: {total}");
                        ++safeRegionSize;
                        if (x == minX || x == maxX -1 || y == minY || y == maxY - 1)
                        {
                            Console.WriteLine($"Error: not considering wide enough area. extreme point {x},{y} qualifies so further points may too.");
                        }
                    }
                }
            }
            Console.WriteLine($"Safe size {safeRegionSize}");
        }
    }
}
