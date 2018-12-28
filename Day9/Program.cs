using System;
using System.Linq;
using System.Diagnostics;
using Common;

namespace Day9
{
    class Program
    {
        static long MarbleGame(int playerCount, int lastMarble)
        {
            long[] scores = new long[playerCount];
            CircularQueue<int> marbles = new CircularQueue<int>(lastMarble + 1);
            marbles.Enqueue(0);
            int currentMarble = 0;
            int currentPlayer = 0;
            while (currentMarble < lastMarble)
            {
                currentMarble++;
                if (currentMarble % 23 != 0)
                {
                    marbles.SkipForward();
                    marbles.Enqueue(currentMarble);
                }
                else
                {
                    scores[currentPlayer] += currentMarble;
                    for (int i = 0; i < 7; ++i)
                    {
                        marbles.SkipBackward();
                    }
                    scores[currentPlayer] += marbles.Dequeue();
                    marbles.SkipForward();
                }
                ++currentPlayer;
                if (currentPlayer >= playerCount)
                {
                    currentPlayer = 0;
                }
            }
            return scores.Max();
        }
        static void Main(string[] args)
        {
            Debug.Assert(MarbleGame(9, 25) == 32);
            Debug.Assert(MarbleGame(10, 1618) == 8317);
            Debug.Assert(MarbleGame(13, 7999) == 146373);
            Debug.Assert(MarbleGame(17, 1104) == 2764);
            Debug.Assert(MarbleGame(21, 6111) == 54718);
            Debug.Assert(MarbleGame(30, 5807) == 37305);
            Console.WriteLine($"Winning score is {MarbleGame(459, 71320)}");
            Console.WriteLine($"Winning score is {MarbleGame(459, 71320 * 100)}");
        }
    }
}
