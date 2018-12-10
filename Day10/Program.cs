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
                        Console.Write('.');
                    }
                }
                Console.WriteLine("");
            }
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

            int time = 0;
            int maxDuration = 100000;
            IEnumerable<Particle> particles = initialParticles;
            int step = 100;
            int prevH = int.MaxValue;
            int prevW = int.MaxValue;
            while (time < maxDuration)
            {
                time += step;
                particles = Step(particles, step);
                var bounds = Bounds(particles);
                var w = bounds.maxX - bounds.minX;
                var h = bounds.maxY - bounds.minY;
                Console.WriteLine($"At time {time}, the bounding box size of all particles is: {w} x {h}");
                if (h > 600)
                {
                    step = 100;
                } else if (h > 500)
                {
                    step = 5;
                } else
                {
                    step = 1;
                }
                if (h > prevH || w > prevW || h < 15)
                {
                    OutputParticles(particles);
                }
                prevW = w;
                prevH = h;
            }

            Console.WriteLine($"Part one: ");
        }
    }
}
