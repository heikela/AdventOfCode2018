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
            const int fabricSize = 1000;
            var lines = File.ReadLines("../../../input.txt");
            HashSet<int>[,] fabric = new HashSet<int>[fabricSize, fabricSize];
            string pattern = "#(\\d+) @ (\\d+),(\\d+): (\\d+)x(\\d+)";
            HashSet<int> idsWithoutConflicts = new HashSet<int>();
            foreach (string line in lines)
            {
                var match = Regex.Match(line, pattern);
                int id = int.Parse(match.Groups[1].Value);
                int x = int.Parse(match.Groups[2].Value);
                int y = int.Parse(match.Groups[3].Value);
                int w = int.Parse(match.Groups[4].Value);
                int h = int.Parse(match.Groups[5].Value);
                idsWithoutConflicts.Add(id);
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
            foreach (HashSet<int> claims in fabric)
            {
                if (claims != null && claims.Count() > 1)
                {
                    duplicated++;
                    idsWithoutConflicts.ExceptWith(claims);
                }
            }
            Console.WriteLine($"There are {duplicated} squares used by more than one plan");

            Console.WriteLine($"The claim with no overlapping claims has ID: {idsWithoutConflicts.First()}.");

        }
    }
}
