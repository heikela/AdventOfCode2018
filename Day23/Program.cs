using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Common;

namespace Day23
{
    class HalfSpace
    {
        private int NX;
        private int NY;
        private int NZ;
        internal int C;
        public HalfSpace(int nX, int nY, int nZ, int c)
        {
            NX = nX;
            NY = nY;
            NZ = nZ;
            C = c;
        }
        public bool IsWithin(IntPoint3D pos)
        {
            return NX * pos.X + NY * pos.Y + NZ * pos.Z <= C;
        }
        public int Dist(IntPoint3D pos)
        {
            return NX * pos.X + NY * pos.Y + NZ * pos.Z - C;
        }
        public static HalfSpace FromNormalAndPoint(int nX, int nY, int nZ, IntPoint3D point, int margin = 0)
        {
            return new HalfSpace(nX, nY, nZ,
                nX * point.X + nY * point.Y + nZ * point.Z + margin);
        }
        public void ShrinkToPointMargin(IntPoint3D point, int margin)
        {
            int pc = NX * point.X + NY * point.Y + NZ * point.Z;
            if (pc + margin < C)
            {
                C = pc + margin;
            }
        }
    }

    class RangePrism
    {
        List<HalfSpace> Faces;
        public RangePrism(IntPoint3D botPos, int botRange)
        {
            Faces = new List<HalfSpace>();
            Faces.Add(HalfSpace.FromNormalAndPoint(1, 1, 1, botPos, botRange));
            Faces.Add(HalfSpace.FromNormalAndPoint(1, 1, -1, botPos, botRange));
            Faces.Add(HalfSpace.FromNormalAndPoint(1, -1, 1, botPos, botRange));
            Faces.Add(HalfSpace.FromNormalAndPoint(1, -1, -1, botPos, botRange));
            Faces.Add(HalfSpace.FromNormalAndPoint(-1, 1, 1, botPos, botRange));
            Faces.Add(HalfSpace.FromNormalAndPoint(-1, 1, -1, botPos, botRange));
            Faces.Add(HalfSpace.FromNormalAndPoint(-1, -1, 1, botPos, botRange));
            Faces.Add(HalfSpace.FromNormalAndPoint(-1, -1, -1, botPos, botRange));
        }
        public int Dist(IntPoint3D pos)
        {
            return Faces.Select(f => f.Dist(pos)).Max();
        }
        public void ShrinkToPointMargin(IntPoint3D point, int margin)
        {
            foreach (HalfSpace face in Faces)
            {
                face.ShrinkToPointMargin(point, margin);
            }
        }
        public void PrintDiameters()
        {
            Console.WriteLine($"ppp: {Faces[0].C + Faces[7].C}, ppn: {Faces[1].C + Faces[6].C}, pnp: {Faces[2].C + Faces[5].C}, pnn: {Faces[3].C + Faces[4].C}");
        }
    }

    class PrismSearch
    {
        private List<(IntPoint3D pos, int range)> Bots;
        private HashSet<int> Remaining;
        private HashSet<int> Rejected;
        private RangePrism Current;
        private RangePrism Best;
        private Random Random;
        public PrismSearch(IEnumerable<(IntPoint3D pos, int range)> bots)
        {
            Bots = bots.ToList();
            Remaining = new HashSet<int>(Enumerable.Range(0,Bots.Count));
            Rejected = new HashSet<int>();
            Random = new Random();
            var botIdx = Remaining.ElementAt(Random.Next(0, Remaining.Count));
            Remaining.Remove(botIdx);
            var bot = Bots[botIdx];
            Current = new RangePrism(bot.pos, bot.range);
        }

        public (int inRange, int dist) Find()
        {
            while (Remaining.Count > 0)
            {
                var botIdx = Remaining.ElementAt(Random.Next(0, Remaining.Count));
                Remaining.Remove(botIdx);
                var candidate = Bots[botIdx];
                if (Current.Dist(candidate.pos) > candidate.range)
                {
                    Rejected.Add(botIdx);
                } else
                {
                    Current.ShrinkToPointMargin(candidate.pos, candidate.range);
                }
            }
            return (Bots.Count - Rejected.Count, Current.Dist(new IntPoint3D(0, 0, 0)));
        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("input.txt");
            string pattern = "pos=<(-?\\d+),(-?\\d+),(-?\\d+)>, r=(-?\\d+)";
            var bots = lines.Select(l =>
            {
                var match = Regex.Match(l, pattern);
                return (pos: new IntPoint3D(
                    int.Parse(match.Groups[1].Value),
                    int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value)),
                    range: int.Parse(match.Groups[4].Value)
                );
            });
            var interesting = bots.OrderByDescending(b => b.range).First();
            var inRange = 0;
            foreach ((IntPoint3D pos, int range) b in bots)
            {
                if (interesting.pos.ManhattanDist(b.pos) <= interesting.range)
                {
                    ++inRange;
                }
            }
            Console.WriteLine($"{inRange} bots in range of one of them.");

            int mostInRange = 0;
            int shortestDist = int.MaxValue;
            const int randomTrials = 100;
            for (int i = 0; i < randomTrials; ++i)
            {
                var search = new PrismSearch(bots);
                var (count, dist) = search.Find();
                if (count > mostInRange)
                {
                    mostInRange = count;
                    shortestDist = dist;
                } else if (count  == mostInRange && dist < shortestDist)
                {
                    shortestDist = dist;
                }
            }
            Console.WriteLine(shortestDist);
        }
    }
}
