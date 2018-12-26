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
            Dictionary<(int x, int y), HashSet<int>> fabric = new Dictionary<(int x, int y), HashSet<int>>();
            string pattern = "#(\\d+) @ (\\d+),(\\d+): (\\d+)x(\\d+)";
            HashSet<int> idsWithoutConflicts = new HashSet<int>();
            foreach (string line in lines)
            {
                var match = Regex.Match(line, pattern);
                int id = int.Parse(match.Groups[1].Value);
                int l = int.Parse(match.Groups[2].Value);
                int t = int.Parse(match.Groups[3].Value);
                int w = int.Parse(match.Groups[4].Value);
                int h = int.Parse(match.Groups[5].Value);
                idsWithoutConflicts.Add(id);
                for (var x = l; x < l + w; ++x)
                {
                    for (var y = t; y < t + h; ++y)
                    {
                        if (!fabric.ContainsKey((x,y)))
                        {
                            fabric.Add((x, y), new HashSet<int>());
                        }
                        fabric[(x, y)].Add(id);
                    }
                }
            }

            int duplicated = 0;
            foreach (HashSet<int> claims in fabric.Values)
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
