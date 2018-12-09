using System;
using System.Linq;
using System.Diagnostics;

namespace Day9
{
    class MarbleBuffer
    {
        private int[] arr;
        private int head;
        private int end;
        public int Count
        {
            get
            {
                return (head - end) % arr.Length;
            }
        }
        public MarbleBuffer(int maxItems)
        {
            arr = new int[maxItems];
            head = 0;
            end = 0;
        }
        public void SkipForward()
        {
            if (head != end)
            {
                arr[end] = arr[head];
                end = IncCursor(end);
                head = IncCursor(head);
            }
        }
        public void SkipBackward()
        {
            if (head != end)
            {
                end = DecCursor(end);
                head = DecCursor(head);
                arr[head] = arr[end];
            }
        }
        public void Enqueue(int item)
        {
            arr[end] = item;
            end = IncCursor(end);
            if (Count == 0)
            {
                throw new InvalidOperationException("Buffer out of space");
            }
        }
        public int Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("No item to dequeue");
            }
            end = DecCursor(end);
            return arr[end];
        }
        private int IncCursor(int cursor)
        {
            ++cursor;
            if (cursor == arr.Length)
            {
                cursor = 0;
            }
            return cursor;
        }
        private int DecCursor(int cursor)
        {
            --cursor;
            if (cursor < 0)
            {
                cursor = arr.Length - 1;
            }
            return cursor;
        }
    }

    class Program
    {
        static long MarbleGame(int playerCount, int lastMarble)
        {
            long[] scores = new long[playerCount];
            MarbleBuffer marbles = new MarbleBuffer(lastMarble + 1);
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
