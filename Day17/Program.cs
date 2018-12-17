using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Day17
{
    enum TileType
    {
        Sand,
        Clay,
        Water,
        Wet
    }

    struct Area
    {
        public int MinX;
        public int MaxX;
        public int MinY;
        public int MaxY;
        public TileType Type;
        public Area(int minX, int maxX, int minY, int maxY, TileType type)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            Type = type;
        }
        private static readonly string hPattern = "y=(\\d+), x=(\\d+)\\.\\.(\\d+)";
        private static readonly string vPattern = "x=(\\d+), y=(\\d+)\\.\\.(\\d+)";

        public static Area Parse(string line)
        {
            var hMatch = Regex.Match(line, hPattern);
            if (hMatch.Success)
            {
                return new Area(
                    int.Parse(hMatch.Groups[2].Value),
                    int.Parse(hMatch.Groups[3].Value),
                    int.Parse(hMatch.Groups[1].Value),
                    int.Parse(hMatch.Groups[1].Value),
                    TileType.Clay);
            }
            var vMatch = Regex.Match(line, vPattern);
            if (vMatch.Success)
            {
                return new Area(
                    int.Parse(vMatch.Groups[1].Value),
                    int.Parse(vMatch.Groups[1].Value),
                    int.Parse(vMatch.Groups[2].Value),
                    int.Parse(vMatch.Groups[3].Value),
                    TileType.Clay);
            }
            throw new ArgumentException($"Unexpected input {line}");
        }
    }

    class Program
    {
        static Area TopMost(Area a, Area b)
        {
            return a.MinY <= b.MinY ? a : b;
        }
        static Area LeftMost(Area a, Area b)
        {
            return a.MinX <= b.MinX ? a : b;
        }
        static Area RightMost(Area a, Area b)
        {
            return a.MaxX >= b.MaxY ? a : b;
        }
        static void Main(string[] args)
        {
            var lines = File.ReadLines("../../../input.txt");
            var obstacles = new HashSet<Area>(lines.Select(l => Area.Parse(l)));
            HashSet<(int x, int y)> heads = new HashSet<(int, int)>();
            heads.Add((500, 0));
            int minY = obstacles.Select(o => o.MinY).Min();
            int maxY = obstacles.Select(o => o.MaxY).Max();
            int minX = obstacles.Select(o => o.MinX).Min();
            int maxX = obstacles.Select(o => o.MaxX).Max();

            var ground = new TileType[maxX + 1, maxY + 1];

            while (heads.Any())
            {
                var currentHead = heads.First();
                var x = currentHead.x;
                heads.Remove(currentHead);
                var below = obstacles
                    .Where(o => o.MinX <= x && o.MaxX >= x && o.MaxY > currentHead.y);
                if (below.Any())
                {
                    var bottom = below.Aggregate(TopMost);
                    TileType type = TileType.Water;
                    // Fill one row
                    while (type == TileType.Water)
                    {
                        // TODO probably need to somehow aggregate adjacent clay and water as bottom
                        int y = bottom.MinY + 1;
                        var rowMinX = bottom.MinX - 1;
                        var rowMaxX = bottom.MaxX + 1;
                        var left = obstacles
                            .Where(o => o.MinY <= y && o.MaxY >= y && o.MaxX < x);
                        if (left.Any())
                        {
                            Area leftEdge = left.Aggregate(RightMost);
                            if (leftEdge.MaxX >= rowMinX)
                            {
                                rowMinX = leftEdge.MaxX + 1;
                            }
                            else
                            {
                                type = TileType.Wet;
                                heads.Add((rowMinX, y));
                            }
                        }
                        var right = obstacles
                            .Where(o => o.MinY <= y && o.MaxY >= y && o.MinX > x);
                        if (right.Any())
                        {
                            Area rightEdge = right.Aggregate(LeftMost);
                            if (rightEdge.MinX <= rowMaxX)
                            {
                                rowMaxX = rightEdge.MinX - 1;
                            }
                            else
                            {
                                type = TileType.Wet;
                                heads.Add((rowMaxX, y));
                            }
                        }
                        if (type == TileType.Water)
                        {
                            obstacles.Add(new Area(rowMinX, rowMaxX, y, y, type));
                        } else
                        {

                        }
                    }


                }
            }
            int wetSquares = 0;
            Console.WriteLine($"Water can reach {wetSquares} tiles");
        }
    }
}
