using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day6
{
    class Program
    {
        static int Dist((int x, int y) p1, (int x, int y) p2)
        {
            return Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);
        }

        static void ExcludeFromSafest(HashSet<int> safest, IEnumerable<int> toExclude)
        {
            safest.ExceptWith(toExclude);
        }

        static void Main(string[] args)
        {
            var lines = File.ReadLines("../../../input.txt");
            var coords = lines
                .Select(l => l.Split(',').Select(int.Parse))
                .Select(xySequence => (x: xySequence.First(), y: xySequence.Skip(1).First()));
            var minX = coords.Select(point => point.x).Min() - 1;
            var maxX = coords.Select(point => point.x).Max() + 1;
            var minY = coords.Select(point => point.y).Min() - 1;
            var maxY = coords.Select(point => point.y).Max() + 1;

            Dictionary<(int x, int y), int> closestCoord = new Dictionary<(int x, int y), int>();
            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    int i = 0;
                    int minDist = int.MaxValue;
                    int minId = -1;
                    foreach (var xy in coords)
                    {
                        int dist = Dist(xy, (x, y));
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
                    closestCoord.Add((x, y), minId);
                }
            }
            var potentialSafest = new HashSet<int>(Enumerable.Range(0, coords.Count()));
            ExcludeFromSafest(potentialSafest, Enumerable.Range(minY, maxY - minY + 1).Select(y => closestCoord[(minX, y)]));
            ExcludeFromSafest(potentialSafest, Enumerable.Range(minY, maxY - minY + 1).Select(y => closestCoord[(maxX, y)]));
            ExcludeFromSafest(potentialSafest, Enumerable.Range(minX, maxX - minX + 1).Select(x => closestCoord[(x, minY)]));
            ExcludeFromSafest(potentialSafest, Enumerable.Range(minX, maxX - minX + 1).Select(x => closestCoord[(x, maxY)]));

            var winCounts = new Dictionary<int, int>(Enumerable.Range(0, coords.Count()).Select(i => new KeyValuePair<int, int>(i, 0)));

            foreach (int closest in closestCoord.Values)
            {
                if (potentialSafest.Contains(closest))
                {
                    winCounts[closest]++;
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

            Console.WriteLine($"If coords are bad, the safest of them is alone in a region of size {maxClosest}");

            const int maxDistanceSum = 10000;
            int coordCount = coords.Count();
            int marginRequired = maxDistanceSum / coordCount;
            maxX += marginRequired;
            minX -= marginRequired;
            maxY += marginRequired;
            minY -= marginRequired;
            int goodRegionSize = 0;
            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    int total = coords.Select(c => Dist(c, (x, y))).Sum();
                    if (total < maxDistanceSum)
                    {
                        ++goodRegionSize;
                        if (x == minX || x == maxX -1 || y == minY || y == maxY - 1)
                        {
                            Console.WriteLine($"Error: not considering wide enough area. extreme point {x},{y} qualifies so further points may too.");
                        }
                    } else
                    {
                        int offBy = total - maxDistanceSum;
                        int offByPerCoord = offBy / coordCount;
                        if (offByPerCoord > 1)
                        {
                            // We know that we can skip this many steps ahead without skipping over parts of the good region
                            y += offByPerCoord - 1;
                        }
                    }
                }
            }
            Console.WriteLine($"Good region size if points are good: {goodRegionSize}");
        }
    }
}
