using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day10
{
    class Program
    {
        static (int x, int y, int vx, int vy) Step((int x, int y, int vx, int vy) prev, int dt)
        {
            return (x: prev.x + prev.vx * dt, y: prev.y + prev.vy * dt, vx: prev.vx, vy: prev.vy);
        }

        static IEnumerable<(int x, int y, int vx, int vy)> Step(IEnumerable<(int x, int y, int vx, int vy)> prev, int dt = 1)
        {
            return prev.Select(particle => Step(particle, dt));
        }

        static (int minX, int minY, int maxX, int maxY) Bounds(IEnumerable<(int x, int y, int vx, int vy)> particles)
        {
            return (
                minX: particles.Select(p => p.x).Min(),
                minY: particles.Select(p => p.y).Min(),
                maxX: particles.Select(p => p.x).Max(),
                maxY: particles.Select(p => p.y).Max()
            );
        }

        static void OutputParticles(IEnumerable<(int x, int y, int vx, int vy)> particles)
        {
            var bb = Bounds(particles);
            for (int y = bb.minY; y <= bb.maxY; ++y)
            {
                var particlesOnLine = particles.Where(p => p.y == y).OrderBy(p => p.x);
//                var nextParticle = particlesOnLine.GetEnumerator();
                for (int x = bb.minX; x <= bb.maxX; ++x)
                {
                    if (particlesOnLine.Any(p => p.x == x))
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
                return (x: x, y: y, vx: vx, vy: vy);
            }).ToList();

            int time = 0;
            int maxDuration = 100000;
            IEnumerable<(int x, int y, int vx, int vy)> particles = initialParticles;
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
