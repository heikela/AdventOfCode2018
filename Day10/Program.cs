using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day10
{
    struct IntPoint2D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public IntPoint2D(int x, int y)
        {
            X = x;
            Y = y;
        }
        public static IntPoint2D operator +(IntPoint2D a, IntPoint2D b)
        {
            return new IntPoint2D(a.X + b.X, a.Y + b.Y);
        }
        public static IntPoint2D operator -(IntPoint2D a, IntPoint2D b)
        {
            return new IntPoint2D(a.X - b.X, a.Y - b.Y);
        }

        public static IntPoint2D operator *(int m, IntPoint2D point)
        {
            return new IntPoint2D(point.X * m, point.Y * m);
        }

        public int ManhattanDist()
        {
            return Math.Abs(X) + Math.Abs(Y);
        }
        public int ManhattanDist(IntPoint2D other)
        {
            return (this - other).ManhattanDist();
        }
    }

    struct Particle
    {
        public IntPoint2D Pos { get; set; }
        public IntPoint2D Vel { get; set; }
        public Particle(IntPoint2D pos, IntPoint2D vel)
        {
            Pos = pos;
            Vel = vel;
        }
        public Particle Step(int dt = 1)
        {
            return new Particle(Pos + dt * Vel, Vel);
        }
    }

    public struct Span
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public Span(int min, int max)
        {
            Min = min;
            Max = max;
        }
        public static Span Inclusive(int a, int b)
        {
            if (a < b)
            {
                return new Span(a, b);
            }
            return new Span(b, a);
        }
        public static Span Inclusive(double a, double b)
        {
            if (a < b)
            {
                return new Span((int)Math.Floor(a), (int)Math.Ceiling(b));
            }
            return new Span((int)Math.Floor(b), (int)Math.Ceiling(a));
        }
        public static readonly Span Empty = new Span(int.MaxValue, int.MinValue);
        public static readonly Span Maximal = new Span(int.MinValue, int.MaxValue);
        public bool IsEmpty()
        {
            return Max < Min;
        }
        public bool Contains(int x)
        {
            return x >= Min && x <= Max;
        }
        public Span Intersect(Span other)
        {
            return new Span(Math.Max(Min, other.Min), Math.Min(Max, other.Max));
        }
    }

    class Program
    {
        static IEnumerable<Particle> Step(IEnumerable<Particle> prev, int dt = 1)
        {
            return prev.Select(particle => particle.Step(dt));
        }

        static (int minX, int minY, int maxX, int maxY) Bounds(IEnumerable<Particle> particles)
        {
            return (
                minX: particles.Select(p => p.Pos.X).Min(),
                minY: particles.Select(p => p.Pos.Y).Min(),
                maxX: particles.Select(p => p.Pos.X).Max(),
                maxY: particles.Select(p => p.Pos.Y).Max()
            );
        }

        static void OutputParticles(IEnumerable<Particle> particles)
        {
            var bb = Bounds(particles);
            for (int y = bb.minY; y <= bb.maxY; ++y)
            {
                var particlesOnLine = particles.Where(p => p.Pos.Y == y);
//                var nextParticle = particlesOnLine.GetEnumerator();
                for (int x = bb.minX; x <= bb.maxX; ++x)
                {
                    if (particlesOnLine.Any(p => p.Pos.X == x))
                    {
                        Console.Write('#');
                    } else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine("");
            }
        }

        /* solve for x in equation k1 * x + c1 = k2 * x + c2 */
        static double Intercept(int k1, int c1, int k2, int c2)
        {
            return ((double)c2 - c1) / (k1 - k2);
        }

        static void Main(string[] args)
        {
            var lines = File.ReadLines("../../../input.txt");
            var lineformat = "position=< *(-?\\d+), *(-?\\d+)> velocity=< *(-?\\d+),  *(-?\\d+)>";
            var initialParticles = lines.Select(l =>
            {
                var match = Regex.Match(l, lineformat);
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);
                int vx = int.Parse(match.Groups[3].Value);
                int vy = int.Parse(match.Groups[4].Value);
                return new Particle(new IntPoint2D(x, y), new IntPoint2D(vx, vy));
            }).ToList();

            /* The examples shown look like the lettering is around 10 characters tall.
             * It wasn't explicitly stated that that all given stars/particles participate in the pattern.
             * As a heuristic based on these, I'll look for a time step when 10 randomly selected particles
             * are within 15 Y coordinate steps of each other (pairwise) simultaneously */

            int particleCount = initialParticles.Count;
            const int particlesToAlign = 10;
            const int distanceThreshold = 15;
            HashSet<int> randomIndices = new HashSet<int>();
            var random = new Random();
            while (randomIndices.Count < particlesToAlign)
            {
                randomIndices.Add(random.Next(particleCount));
            }
            var particlePairs = randomIndices.SelectMany(
                i => randomIndices.SelectMany(
                    j => {
                        if (i == j)
                        {
                            return new List<(Particle, Particle)>();
                        }
                        return new List<(Particle, Particle)>() { (initialParticles[i], initialParticles[j])};
                    })
                );

            var feasibleTimeSpans = particlePairs.Select(pair => {
                if (pair.Item1.Vel.Y == pair.Item2.Vel.Y)
                {
                    if (Math.Abs(pair.Item1.Pos.Y - pair.Item2.Pos.Y) <= distanceThreshold)
                    {
                        return Span.Maximal;
                    }
                    else
                    {
                        return Span.Empty;
                    }
                }
                else
                {
                    var t1 = Intercept(
                        pair.Item1.Vel.Y, pair.Item1.Pos.Y - distanceThreshold,
                        pair.Item2.Vel.Y, pair.Item2.Pos.Y);
                    var t2 = Intercept(
                        pair.Item1.Vel.Y, pair.Item1.Pos.Y + distanceThreshold,
                        pair.Item2.Vel.Y, pair.Item2.Pos.Y);
                    return Span.Inclusive(t1, t2);
                }
            });

            var bestSpan = feasibleTimeSpans.Aggregate((a, b) => a.Intersect(b));

            for (int time = bestSpan.Min; time <= bestSpan.Max; ++time)
            {
                var particles = Step(initialParticles, time);
                Console.WriteLine($"At time {time}, we see:");
                OutputParticles(particles);
                Console.WriteLine();
            }
        }
    }
}
