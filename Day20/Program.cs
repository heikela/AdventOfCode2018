using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Day20
{
    class Program
    {
        static void ParseInput((int x, int y) start, IEnumerator<char> source, Dictionary<(int x, int y), HashSet<(int x, int y)>> adjacency)
        {
            (int x, int y) current = start;
            while (true)
            {
                source.MoveNext();
                if (!adjacency.ContainsKey(current))
                {
                    adjacency.Add(current, new HashSet<(int x, int y)>());
                }
                switch (source.Current)
                {
                    case '^': break;
                    case 'N':
                        {
                            (int x, int y) next = (current.x, current.y - 1);
                            adjacency[current].Add(next);
                            current = next;
                            break;
                        }
                    case 'E':
                        {
                            (int x, int y) next = (current.x + 1, current.y);
                            adjacency[current].Add(next);
                            current = next;
                            break;
                        }
                    case 'W':
                        {
                            (int x, int y) next = (current.x - 1, current.y);
                            adjacency[current].Add(next);
                            current = next;
                            break;
                        }
                    case 'S':
                        {
                            (int x, int y) next = (current.x, current.y + 1);
                            adjacency[current].Add(next);
                            current = next;
                            break;
                        }
                    case '(':
                        ParseInput(current, source, adjacency);
                        break;
                    case '|':
                        current = start;
                        break;
                    case '$':
                    case ')': return;
                }
            }
        }

        static int DFSDepth((int x, int y) start, Dictionary<(int x, int y), HashSet<(int x, int y)>> adjacency)
        {
            HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();
            HashSet<(int x, int y)> frontier = new HashSet<(int x, int y)>();
            frontier.Add(start);
            int dist = 0;
            while (frontier.Except(visited).Any())
            {
                visited.UnionWith(frontier);
                frontier = new HashSet<(int x, int y)>(frontier.SelectMany(pos => adjacency[pos]));
                frontier.ExceptWith(visited);
                ++dist;
            }
            return dist - 1;
        }

        static int DFSFurtherThan1000((int x, int y) start, Dictionary<(int x, int y), HashSet<(int x, int y)>> adjacency)
        {
            HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();
            HashSet<(int x, int y)> frontier = new HashSet<(int x, int y)>();
            HashSet<(int x, int y)> farAway = new HashSet<(int x, int y)>();
            frontier.Add(start);
            int dist = 0;
            while (frontier.Except(visited).Any())
            {
                visited.UnionWith(frontier);
                frontier = new HashSet<(int x, int y)>(frontier.SelectMany(pos => adjacency[pos]));
                frontier.ExceptWith(visited);
                ++dist;
                if (dist >= 1000)
                {
                    farAway.UnionWith(frontier);
                }
            }
            return farAway.Count();
        }

        static void Main(string[] args)
        {
            Dictionary<(int x, int y), HashSet<(int x, int y)>> maze1 = new Dictionary<(int x, int y), HashSet<(int x, int y)>>();
            Dictionary<(int x, int y), HashSet<(int x, int y)>> maze2 = new Dictionary<(int x, int y), HashSet<(int x, int y)>>();
            Dictionary<(int x, int y), HashSet<(int x, int y)>> maze3 = new Dictionary<(int x, int y), HashSet<(int x, int y)>>();
            string input = File.ReadLines("../../../input.txt").First();
            string sampleInput = "^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$";
            ParseInput((0, 0), "^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$".GetEnumerator(), maze1);
            ParseInput((0, 0), sampleInput.GetEnumerator(), maze2);
            ParseInput((0, 0), input.GetEnumerator(), maze3);
            Console.WriteLine(DFSDepth((0, 0), maze1));
            Console.WriteLine(DFSDepth((0, 0), maze2));
            Console.WriteLine(DFSDepth((0, 0), maze3));
            Console.WriteLine(DFSFurtherThan1000((0, 0), maze3));
        }
    }
}
