using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Day25
{
    class Program
    {
        static int CCount(string filename)
        {
            var input = File.ReadLines(filename);
            var points = input.Select(l => l.Split(',').Select(num => int.Parse(num)).ToArray()).ToList();
            List<HashSet<int>> adjacency = new List<HashSet<int>>(Enumerable.Range(0, points.Count).Select(i => new HashSet<int>()));
            for (int i = 0; i < points.Count; ++i)
            {
                for (int j = i + 1; j < points.Count; ++j)
                {
                    int dist = 0;
                    for (int d = 0; d < 4; ++d)
                    {
                        dist += Math.Abs(points[i][d] - points[j][d]);
                    }
                    if (dist <= 3)
                    {
                        adjacency[i].Add(j);
                        adjacency[j].Add(i);
                    }
                }
            }
            Dictionary<int, int> constellations = new Dictionary<int, int>();
            for (int i = 0; i < points.Count; ++i)
            {
                HashSet<int> frontier = new HashSet<int>() { i };
                frontier.ExceptWith(constellations.Keys);
                foreach (int p in frontier)
                {
                    constellations.Add(p, i);
                }
                while (frontier.Any())
                {
                    frontier = new HashSet<int>(frontier.SelectMany(p => adjacency[p]));
                    frontier.ExceptWith(constellations.Keys);
                    foreach (int p in frontier)
                    {
                        constellations.Add(p, i);
                    }
                }
            }
            int ccount = constellations.Values.Distinct().Count();
            return ccount;
        }
        static void Main(string[] args)
        {
            Debug.Assert(CCount("../../../sampleInput1.txt") == 2);
            Debug.Assert(CCount("../../../sampleInput2.txt") == 4);

            var inputFile = args.Length >= 1 ? args[0] : "../../../input.txt";
            var result = CCount(inputFile);
            Console.WriteLine(result);
        }
    }
}
