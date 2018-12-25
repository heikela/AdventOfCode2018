using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

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

    class WaterSim {
        private HashSet<(int x, int y)> Heads;
        private int MinY;
        private int MaxY;
        private int MinX;
        private int MaxX;
        private TileType[,] Ground;
        public WaterSim(IEnumerable<string> lines)
        {
            Heads = new HashSet<(int, int)>();
            Heads.Add((500, 0));
            var obstacles = new HashSet<Area>(lines.Select(l => Area.Parse(l)));
            MinY = obstacles.Select(o => o.MinY).Min();
            MaxY = obstacles.Select(o => o.MaxY).Max();
            MinX = obstacles.Select(o => o.MinX).Min() - 1;
            MaxX = obstacles.Select(o => o.MaxX).Max() + 1;

            Ground = new TileType[MaxX + 1, MaxY + 1];
            foreach (Area o in obstacles)
            {
                for (int y = o.MinY; y <= o.MaxY; ++y)
                {
                    for (int x = o.MinX; x <= o.MaxX; ++x)
                    {
                        Ground[x, y] = o.Type;
                    }
                }
            }

        }

        private bool Permeable(int x, int y)
        {
            return Ground[x, y] == TileType.Sand || Ground[x, y] == TileType.Wet;
        }

        private bool Permeable((int x, int y) coords)
        {
            return Permeable(coords.x, coords.y);
        }

        public void SaveImage(string filename)
        {
            var image = new Bitmap(MaxX + 1, MaxY + 1);
            for (int y = 0; y <= MaxY; ++y)
            {
                for (int x = 0; x <= MaxX; ++x)
                {
                    switch (Ground[x, y])
                    {
                        case TileType.Clay:
                            image.SetPixel(x, y, Color.FromName("Black"));
                            break;
                        case TileType.Sand:
                            image.SetPixel(x, y, Color.FromName("SandyBrown"));
                            break;
                        case TileType.Wet:
                            image.SetPixel(x, y, Color.FromName("LightBlue"));
                            break;
                        case TileType.Water:
                            image.SetPixel(x, y, Color.FromName("DarkBlue"));
                            break;
                    }
                }
            }
            image.Save(filename);
        }

        public void Solve()
        {
            while (Heads.Any())
            {
                var currentHead = Heads.First();
                Heads.Remove(currentHead);
                while (currentHead.y < MaxY && Ground[currentHead.x, currentHead.y + 1] == TileType.Sand)
                {
                    currentHead = (currentHead.x, currentHead.y + 1);
                    Ground[currentHead.x, currentHead.y] = TileType.Wet;
                }
                if (currentHead.y == MaxY || Ground[currentHead.x, currentHead.y + 1] == TileType.Wet)
                {
                    continue;
                }
                bool enclosed = true;
                var left = (x: currentHead.x - 1, y: currentHead.y);
                while (Permeable(left) && !Permeable(left.x, left.y + 1))
                {
                    Ground[left.x, left.y] = TileType.Wet;
                    left = (x: left.x - 1, y: left.y);
                }
                if (Permeable(left))
                {
                    enclosed = false;
                }
                if (Ground[left.x, left.y + 1] == TileType.Sand)
                {
                    Ground[left.x, left.y] = TileType.Wet;
                    Heads.Add(left);
                }
                var right = (x: currentHead.x + 1, y: currentHead.y);
                while (Permeable(right) && !Permeable(right.x, right.y + 1))
                {
                    Ground[right.x, right.y] = TileType.Wet;
                    right = (x: right.x + 1, y: right.y);
                }
                if (Permeable(right))
                {
                    enclosed = false;
                }
                if (Ground[right.x, right.y + 1] == TileType.Sand)
                {
                    Ground[right.x, right.y] = TileType.Wet;
                    Heads.Add(right);
                }
                if (enclosed)
                {
                    for (int x = left.x + 1; x < right.x; ++x)
                    {
                        Ground[x, currentHead.y] = TileType.Water;
                    }
                    if (Permeable(currentHead.x, currentHead.y - 1))
                    {
                        Ground[currentHead.x, currentHead.y - 1] = TileType.Wet;
                        Heads.Add((x: currentHead.x, y: currentHead.y - 1));
                    }
                }
            }
        }

        public int Wet()
        {
            int wet = 0;
            for (int y = MinY; y <= MaxY; ++y)
            {
                for (int x = MinX; x <= MaxX; ++x)
                {
                    if (Ground[x, y] == TileType.Water || Ground[x, y] == TileType.Wet)
                    {
                        ++wet;
                    }
                }
            }
            return wet;
        }

        public int StandingWater()
        {
            int wet = 0;
            for (int y = MinY; y <= MaxY; ++y)
            {
                for (int x = MinX; x <= MaxX; ++x)
                {
                    if (Ground[x, y] == TileType.Water)
                    {
                        ++wet;
                    }
                }
            }
            return wet;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("input.txt");
            var puzzle = new WaterSim(lines);
            puzzle.Solve();

            int wetSquares = puzzle.Wet();
            Console.WriteLine($"Water can reach {wetSquares} tiles");
            puzzle.SaveImage("output.png");
            int waterSquares = puzzle.StandingWater();
            Console.WriteLine($"Standing water can be found in {waterSquares} tiles");

        }
    }
}
