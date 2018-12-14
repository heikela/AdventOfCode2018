using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day14
{
    class ChocolateFactory {
        private List<int> Scoreboard;
        private List<int> ElfIdx;

        public ChocolateFactory()
        {
            Scoreboard = new List<int>() { 3, 7 };
            ElfIdx = new List<int>() { 0, 1 };
        }
        public void Experiment()
        {
            int sum = ElfIdx.Select(i => Scoreboard[i]).Sum();
            if (sum >= 10)
            {
                Scoreboard.Add(sum / 10);
                sum -= 10 * (sum / 10);
            }
            Scoreboard.Add(sum);
            for (int i = 0; i < ElfIdx.Count; ++i)
            {
                int steps = Scoreboard[ElfIdx[i]] + 1;
                ElfIdx[i] += steps;
                if (ElfIdx[i] >= Scoreboard.Count)
                {
                    ElfIdx[i] %= Scoreboard.Count;
                }
            }
        }
        public bool HaveScoreAfter(int priorRecipes)
        {
            return Scoreboard.Count >= priorRecipes + 10;
        }
        public string ScoreAfter(int priorRecipes)
        {
            return Scoreboard.Skip(priorRecipes).Take(10).Aggregate("", (prev, score) => prev + score);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int input = 793031;
            var factory = new ChocolateFactory();
            while (!factory.HaveScoreAfter(input))
            {
                factory.Experiment();
            }
            string scoreAfter = factory.ScoreAfter(input);
            factory = new ChocolateFactory();
            int inputIndex = -1;
            int i = 0;
            string inputString = "793031";
            while (inputIndex < 0)
            {
                while (!factory.HaveScoreAfter(i))
                {
                    factory.Experiment();
                }
                while (factory.HaveScoreAfter(i))
                {
                    string possible = factory.ScoreAfter(i).Substring(0, inputString.Length);
                    if (possible == inputString)
                    {
                        inputIndex = i;
                        break;
                    }
                    ++i;
                }
            }
            Console.WriteLine($"Found {inputString} after {inputIndex} recipes.");
        }
    }
}
