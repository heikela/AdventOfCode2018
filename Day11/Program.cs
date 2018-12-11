using System;
using System.Diagnostics;

namespace Day11
{
    class Program
    {
        readonly static int end = 301;
        readonly static int start = 1;

        static int [,] MakeGrid(int serial)
        {
            const int end = 301;
            const int start = 1;
            int[,] power = new int[end, end];
            for (int x = start; x < end; ++x)
            {
                for (int y = start; y < end; ++y)
                {
                    int rackId = x + 10;
                    int hundredsFrom = (rackId * y + serial) * rackId;
                    int hundreds = (hundredsFrom % 1000) / 100;
                    power[x, y] = hundreds - 5;
                }
            }
            return power;
        }

        static (int x, int y, int max) FindMaxSquare(int[,] grid, int squareSize = 3)
        {
            int max = int.MinValue;
            int maxX = 0;
            int maxY = 0;
            for (int tlx = start; tlx < end - squareSize + 1; ++tlx)
            {
                for (int tly = start; tly < end - squareSize + 1; ++tly)
                {
                    int sum = 0;
                    for (int x = tlx; x < tlx + squareSize; ++x)
                    {
                        for (int y = tly; y < tly + squareSize; ++y)
                        {
                            sum += grid[x, y];
                        }
                    }
                    if (sum > max)
                    {
                        maxX = tlx;
                        maxY = tly;
                        max = sum;
                    }
                }
            }
            return (x: maxX, y: maxY, power: max);
        }

        static void Main(string[] args)
        {
            const int serial = 7403;

            Debug.Assert(MakeGrid(57)[122, 79] == -5);
            Debug.Assert(MakeGrid(39)[217, 196] == 0);
            Debug.Assert(MakeGrid(71)[101, 153] == 4);
            Debug.Assert(FindMaxSquare(MakeGrid(18)) == (x: 33, y: 45, max: 29));
            Debug.Assert(FindMaxSquare(MakeGrid(42)) == (x: 21, y: 61, max: 30));

            var power = MakeGrid(serial);

            var bestSoFar = int.MinValue;
            for (int size = 1; size < end; ++size)
            {
                var (x, y, max) = FindMaxSquare(power, size);
                if (max >= bestSoFar)
                {
                    Console.WriteLine($"Max power at {x},{y},{size} : {max}");
                    bestSoFar = max;
                }
                if (max < -1000)
                {
                    // the numbers average approximately -0.5.
                    // As square size grows, eventually the sums become increasingly negative.
                    // give up search when it seems that this happens.
                    break;
                }
            }
        }
    }
}
