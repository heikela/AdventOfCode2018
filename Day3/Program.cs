using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("../../../input.txt");
            HashSet<int>[,] fabric = new HashSet<int>[1000, 1000];
            string pattern = "#(\\d+) @ (\\d+),(\\d+): (\\d+)x(\\d+)";
            HashSet<int> ids = new HashSet<int>();
            foreach (string line in lines)
            {
                var match = Regex.Match(line, pattern);
                int id = int.Parse(match.Groups[1].Value);
                int x = int.Parse(match.Groups[2].Value);
                int y = int.Parse(match.Groups[3].Value);
                int w = int.Parse(match.Groups[4].Value);
                int h = int.Parse(match.Groups[5].Value);
                ids.Add(id);
                for (var xx = x; xx < x + w; ++xx)
                {
                    for (var yy = y; yy < y + h; ++yy)
                    {
                        if (fabric[xx,yy] == null)
                        {
                            fabric[xx, yy] = new HashSet<int>();
                        }
                        fabric[xx, yy].Add(id);
                    }

                }
            }

            int duplicated = 0;
            for (int x = 0; x < 1000; ++x)
            {
                for (int y = 0; y < 1000; ++y)
                {
                    if (fabric[x,y] != null && fabric[x,y].Count() > 1)
                    {
                        duplicated++;
                    }
                }
            }
            System.Console.WriteLine($"There are {duplicated} squares used by more than one plan");

            for (int x = 0; x < 1000; ++x)
            {
                for (int y = 0; y < 1000; ++y)
                {
                    if (fabric[x, y] != null && fabric[x, y].Count() > 1)
                    {
                        foreach(int id in fabric[x,y])
                        {
                            if (ids.Contains(id))
                            {
                                ids.Remove(id);
                            }
                        }
                    }
                }
            }
            System.Console.WriteLine($"Masterplan ID is {ids.First()}.");

        }
    }
}
