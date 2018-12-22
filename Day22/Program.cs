using System;
using System.Collections.Generic;
using System.Linq;

namespace Day22
{
    class Program
    {
        static void Main(string[] args)
        {
            int depth = 4080;
            int targetX = 14;
            int targetY = 785;

            int[,] gi = new int[targetX + 1, targetY + 1];
            int[,] erosion = new int[targetX + 1, targetY + 1];

            for (int y = 0; y <= targetY; ++y)
            {
                for (int x = 0; x <= targetX; ++x)
                {
                    if (x == 0 && y == 0)
                    {
                        gi[x, y] = 0;
                    } else if (x == targetX && y == targetY)
                    {
                        gi[x, y] = 0;
                    } else if (y == 0)
                    {
                        gi[x, y] = x * 16807;
                    } else if (x == 0)
                    {
                        gi[x, y] = y * 48271;
                    } else
                    {
                        gi[x, y] = erosion[x - 1, y] * erosion[x, y - 1];
                    }
                    erosion[x, y] = (gi[x, y] + depth) % 20183;
                }
            }

            int riskLevel = 0;
            foreach (int e in erosion) {
                riskLevel += e % 3;
            }
            Console.WriteLine($"Risk level = {riskLevel}");
        }
    }
}
