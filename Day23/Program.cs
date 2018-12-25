using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Day23
{
    struct IntPoint3D : IEquatable<IntPoint3D>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public IntPoint3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public static IntPoint3D operator +(IntPoint3D a, IntPoint3D b)
        {
            return new IntPoint3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static IntPoint3D operator -(IntPoint3D a, IntPoint3D b)
        {
            return new IntPoint3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static IntPoint3D operator *(int m, IntPoint3D point)
        {
            return new IntPoint3D(point.X * m, point.Y * m, point.Z * m);
        }

        public override bool Equals(object obj)
        {
            if (obj is IntPoint3D)
            {
                return this.Equals((IntPoint3D)obj);
            }
            return false;
        }
        public bool Equals(IntPoint3D p)
        {
            return X == p.X && Y == p.Y && Z == p.Z;
        }
        public static bool operator ==(IntPoint3D a, IntPoint3D b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(IntPoint3D a, IntPoint3D b)
        {
            return !(a.Equals(b));
        }
        public int ManhattanDist()
        {
            long d = (long)Math.Abs(X) + (long)Math.Abs(Y) + (long)Math.Abs(Z);
            if (d > int.MaxValue)
            {
                return int.MaxValue;
            }
            return (int)d;
        }
        public int ManhattanDist(IntPoint3D other)
        {
            return (this - other).ManhattanDist();
        }
    }

    class Box
    {
        private IntPoint3D MinXYZ;
        private IntPoint3D MaxXYZ;
        private int HalfDiagonal;
        private List<Box> Children;
        private int ConservativeCornerCount;
        private int UpperBound;
        public Box(IntPoint3D minXYZ, IntPoint3D maxXYZ, IEnumerable<(IntPoint3D pos, int range)> bots)
        {
            MinXYZ = minXYZ;
            MaxXYZ = maxXYZ;
            HalfDiagonal = minXYZ.ManhattanDist(MaxXYZ) / 2 + 1;
            Children = new List<Box>();
            var cornerCounts = Corners().Select(c => bots.Count(b => Search.InRange(c, b))).ToList();
            ConservativeCornerCount = cornerCounts.Max();
            UpperBound = bots.Count(bot =>
                Math.Max(0, bot.pos.X - maxXYZ.X) +
                Math.Max(0, minXYZ.X - bot.pos.X) +
                Math.Max(0, bot.pos.Y - maxXYZ.Y) +
                Math.Max(0, minXYZ.Y - bot.pos.Y) +
                Math.Max(0, bot.pos.Z - maxXYZ.Z) +
                Math.Max(0, minXYZ.Z - bot.pos.Z) < bot.range);
        }
        private IEnumerable<IntPoint3D> Corners()
        {
            yield return MinXYZ;
            yield return new IntPoint3D(MinXYZ.X, MinXYZ.Y, MaxXYZ.Z);
            yield return new IntPoint3D(MinXYZ.X, MaxXYZ.Y, MinXYZ.Z);
            yield return new IntPoint3D(MinXYZ.X, MaxXYZ.Y, MaxXYZ.Z);
            yield return new IntPoint3D(MaxXYZ.X, MinXYZ.Y, MinXYZ.Z);
            yield return new IntPoint3D(MaxXYZ.X, MinXYZ.Y, MaxXYZ.Z);
            yield return new IntPoint3D(MaxXYZ.X, MaxXYZ.Y, MinXYZ.Z);
            yield return MaxXYZ;
            yield break;
        }

        private List<int> GetLevelCounts()
        {
            if (Children.Any())
            {
                List<List<int>> childLevelCounts = new List<List<int>>(Children.Select(c => c.GetLevelCounts()));
                int maxDepth = childLevelCounts.Select(l => l.Count).Max();
                return Enumerable.Repeat(1, 1)
                    .Concat(Enumerable
                        .Range(0, maxDepth)
                        .Select(i =>
                            childLevelCounts.Select(c => c.Count > i ? c[i] : 0).Sum())).ToList();
            } else
            {
                return new List<int>() { 1 };
            }
        }

        public void PrintLevelCounts()
        {
            var counts = GetLevelCounts();
            for (int i = 0; i < counts.Count; ++i)
            {
                Console.WriteLine($"Level {i} has {counts[i]} nodes.");
            }
        }

        public int Partition(int relativeDepth, int bestCount, IEnumerable<(IntPoint3D pos, int range)> bots)
        {
            if (relativeDepth > 0 && !Children.Any()) return ConservativeCornerCount;
            if (relativeDepth > 0)
            {
                int best = ConservativeCornerCount;
                foreach (Box c in Children)
                {
                    if (c.UpperBound >= bestCount)
                    {
                        int childResult = c.Partition(relativeDepth - 1, bestCount, bots);
                        if (childResult > best)
                        {
                            best = childResult;
                        }
                    }
                }
                return best;
            }
            if (relativeDepth == 0)
            {
                if (UpperBound >= bestCount)
                {
                    int midx = (MinXYZ.X + MaxXYZ.X) / 2;
                    int midy = (MinXYZ.Y + MaxXYZ.Y) / 2;
                    int midz = (MinXYZ.Z + MaxXYZ.Z) / 2;
                    Children = new List<Box>();
                    Children.Add(new Box(
                        MinXYZ, new IntPoint3D(midx, midy, midz),
                        bots));
                    Children.Add(new Box(
                        new IntPoint3D(MinXYZ.X, MinXYZ.Y, midz),
                        new IntPoint3D(midx, midy, MaxXYZ.Z),
                        bots));
                    Children.Add(new Box(
                        new IntPoint3D(MinXYZ.X, midy, MinXYZ.Z),
                        new IntPoint3D(midx, MaxXYZ.Y, midz),
                        bots));
                    Children.Add(new Box(
                        new IntPoint3D(MinXYZ.X, midy, midz),
                        new IntPoint3D(midx, MaxXYZ.Y, MaxXYZ.Z),
                        bots));
                    Children.Add(new Box(
                        new IntPoint3D(midx, MinXYZ.Y, MinXYZ.Z),
                        new IntPoint3D(MaxXYZ.X, midy, midz),
                        bots));
                    Children.Add(new Box(
                        new IntPoint3D(midx, MinXYZ.Y, midz),
                        new IntPoint3D(MaxXYZ.X, midy, MaxXYZ.Z),
                        bots));
                    Children.Add(new Box(
                        new IntPoint3D(midx, midy, MinXYZ.Z),
                        new IntPoint3D(MaxXYZ.X, MaxXYZ.Y, midz),
                        bots));
                    Children.Add(new Box(
                        new IntPoint3D(midx, midy, midz),
                        MaxXYZ,
                        bots));
                    Children = Children.Where(c => c.UpperBound >= bestCount).ToList();
                }
                int best = ConservativeCornerCount;
                foreach (Box c in Children)
                {
                    int childResult = c.ConservativeCornerCount;
                    if (childResult > best)
                    {
                        best = childResult;
                    }
                }
                return best;
            }
            return 1000000000;
        }
    }

    class Search
    {
        private List<(IntPoint3D pos, int range)> Bots;
        private Box Root;
        private int BestCount;
        public Search(IEnumerable<(IntPoint3D pos, int range)> bots)
        {
            Bots = bots.ToList();
            Root = new Box(
                new IntPoint3D(
                    Bots.Select(b => b.pos.X - b.range).Min(),
                    Bots.Select(b => b.pos.Y - b.range).Min(),
                    Bots.Select(b => b.pos.Z - b.range).Min()),
                new IntPoint3D(
                    Bots.Select(b => b.pos.X + b.range).Max(),
                    Bots.Select(b => b.pos.Y + b.range).Max(),
                    Bots.Select(b => b.pos.Z + b.range).Max()),
                Bots);
        }

        public static bool InRange(IntPoint3D point, (IntPoint3D pos, int range) bot)
        {
            return bot.pos.ManhattanDist(point) < bot.range;
        }
        public IntPoint3D Find()
        {
            int level = 0;

            BestCount = 918;
            while (level < 30)
            {
                var bestFromSearch = Root.Partition(level, BestCount, Bots);
                BestCount = Math.Max(BestCount, bestFromSearch);
                level++;
                Root.PrintLevelCounts();
            }
            return new IntPoint3D(0, 0, 0);
        }
    }

    class RandomSearch
    {
        private List<(IntPoint3D pos, int range)> Bots;
        private List<(IntPoint3D pos, double value)> Swarm;
        private (IntPoint3D pos, double value) Best;
        private int D = 500000000;
        private double T = 10.0;
        const double distScale = 1e-8;

        public double Value(IntPoint3D pos)
        {
            return CountInRange(pos) - distScale * pos.ManhattanDist();
        }

        public RandomSearch(IEnumerable<(IntPoint3D pos, int range)> bots)
        {
            Bots = bots.ToList();
            var origin = new IntPoint3D(0, 0, 0);
            Best = (origin, Value(origin));
            Swarm = new List<(IntPoint3D pos, double value)>(Enumerable.Repeat(Best, 5000));
            D = 500000000;
            T = 500.0;
        }

        public static bool InRange(IntPoint3D point, (IntPoint3D pos, int range) bot)
        {
            int dist = bot.pos.ManhattanDist(point);
            return dist <= bot.range;
        }

        public int CountInRange(IntPoint3D point)
        {
            return Bots.Count(
                b => InRange(point, b));
        }

        public IntPoint3D Find()
        {
            var rand = new Random();
            while (D > 1)
            {
                for (int i = 0; i < 30; ++i)
                {
                    Swarm = Swarm.Select(candidate =>
                    {
                        var newPoint = new IntPoint3D(
                            candidate.pos.X + rand.Next(-D, D),
                            candidate.pos.Y + rand.Next(-D, D),
                            candidate.pos.Z + rand.Next(-D, D));
                        var newValue = Value(newPoint);
                        if (candidate.value < newValue)
                        {
                            return (newPoint, newValue);
                        }
                        else
                        {
                            if (rand.NextDouble() < Math.Exp((candidate.value - newValue) / T))
                            {
                                return (newPoint, newValue);
                            }
                            else
                            {
                                return candidate;
                            }
                        }
                    }).ToList();
                    Best = Swarm.Aggregate(Best, (a, b) => (a.value > b.value) ? a : b);
                    Console.WriteLine($"D = {D}, T = {T}, Best pos = {Best.pos.X},{Best.pos.Y},{Best.pos.Z} value = {Best.value:F8}");
                    Swarm = Swarm.OrderByDescending(candidate => candidate.value).ToList();
                    Swarm = Swarm.Take(500).SelectMany(candidate => Enumerable.Repeat(candidate, 10)).ToList();
                }
                if (D > 1)
                {
                    D /= 2;
                }
                T *= 0.8;
            }
            return Best.pos;
        }
    }

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
        private int BestCount;
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
            Best = Current;
            BestCount = 1;
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
//                    Console.WriteLine($"{Rejected.Count} rejected, {Bots.Count - Rejected.Count - Remaining.Count} reached.");
//                    Current.PrintDiameters();
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
