using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Day18
{
    class LumberSimulation
    {
        private int H;
        private int W;
        private char[,] Grid;

        public LumberSimulation(IEnumerable<string> lines)
        {
            H = lines.Count();
            W = lines.Select(l => l.Length).Max();

            Grid = new char[W, H];

            int y = 0;
            foreach (string l in lines)
            {
                int x = 0;
                foreach (char c in l)
                {
                    Grid[x, y] = c;
                    ++x;
                }
                ++y;
            }
        }

        public void PrintGrid()
        {
            Console.WriteLine();
            for (int y = 0; y < H; ++y)
            {
                for (int x = 0; x < W; ++x)
                {
                    Console.Write(Grid[x, y]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void Next()
        {
            char[,] next = new char[W, H];
            for (int y = 0; y < H; ++y)
            {
                for (int x = 0; x < W; ++x)
                {
                    if (Grid[x,y] == '.')
                    {
                        int wood = 0;
                        for (int yy = Math.Max(0, y - 1); yy < Math.Min(H, y + 2); ++yy)
                        {
                            for (int xx = Math.Max(0, x - 1); xx < Math.Min(W, x + 2); ++xx)
                            {
                                if (Grid[xx,yy] == '|')
                                {
                                    ++wood;
                                }
                            }
                        }
                        if (wood >= 3) {
                            next[x, y] = '|';
                        } else
                        {
                            next[x, y] = '.';
                        }
                    }
                    if (Grid[x, y] == '|')
                    {
                        int lumberYard = 0;
                        for (int yy = Math.Max(0, y - 1); yy < Math.Min(H, y + 2); ++yy)
                        {
                            for (int xx = Math.Max(0, x - 1); xx < Math.Min(W, x + 2); ++xx)
                            {
                                if (Grid[xx, yy] == '#')
                                {
                                    ++lumberYard;
                                }
                            }
                        }
                        if (lumberYard >= 3)
                        {
                            next[x, y] = '#';
                        }
                        else
                        {
                            next[x, y] = '|';
                        }
                    }
                    if (Grid[x, y] == '#')
                    {
                        int wood = 0;
                        int lumberYards = 0;
                        for (int yy = Math.Max(0, y - 1); yy < Math.Min(H, y + 2); ++yy)
                        {
                            for (int xx = Math.Max(0, x - 1); xx < Math.Min(W, x + 2); ++xx)
                            {
                                if (Grid[xx, yy] == '|')
                                {
                                    ++wood;
                                }
                                if (Grid[xx, yy] == '#')
                                {
                                    ++lumberYards;
                                }
                            }
                        }
                        if (wood >= 1 && lumberYards - 1 >= 1)
                        {
                            next[x, y] = '#';
                        }
                        else
                        {
                            next[x, y] = '.';
                        }
                    }
                }
            }
            Grid = next;
        }

        public int Value()
        {
            int wood = 0;
            int lumberYard = 0;
            foreach (char c in Grid)
            {
                if (c == '|')
                {
                    ++wood;
                }
                if (c == '#')
                {
                    ++lumberYard;
                }
            }

            return wood * lumberYard;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var sim = new LumberSimulation(File.ReadLines("../../../input.txt"));
//            var sim = new LumberSimulation(File.ReadLines("../../../sampleInput.txt"));
//            sim.PrintGrid();
            int time = 0;
            while (time < 1000000000)
            {
                ++time;
                sim.Next();
//                sim.PrintGrid();
                Console.WriteLine($"{time}, {sim.Value()}");
            }
        }
    }
}
