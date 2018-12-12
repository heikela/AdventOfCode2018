using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Day12
{
    class Program
    {
        static IEnumerable<bool> NextGen(List<bool> current, Dictionary<(bool, bool, bool, bool, bool), bool> rules)
        {
            for (int i = 2; i < current.Count - 2; ++i)
            {
                var key = (current[i - 2], current[i - 1], current[i], current[i + 1], current[i + 2]);
                if (rules.ContainsKey(key))
                {
                    yield return rules[key];
                }
                else
                {
                    yield return false;
                }
            }
            yield break;
        }

        static void Main(string[] args)
        {
            Dictionary<(bool, bool, bool, bool, bool), bool> rules = new Dictionary<(bool, bool, bool, bool, bool), bool>();
            List<bool> initial = new List<bool>();
            //var input = File.ReadLines("../../../sampleInput.txt").ToList();
            var input = File.ReadLines("../../../input.txt").ToList();
            initial = input[0].Skip(15).Select(c => c == '#').ToList();
            foreach (string line in input.Skip(2)) {
                var bools = line.Take(5).Select(c => c == '#').ToList();
                rules.Add((bools[0], bools[1], bools[2], bools[3], bools[4]), line[9] == '#');
            }

            int leftMost = 0;
            int gen = 0;
            List<bool> current = initial;
            var padding = new List<bool>() { false, false, false, false };
            if (gen < 10)
            {
                Console.Write(' ');
            }
            Console.Write(gen);
            Console.Write(": ");
            foreach (bool b in current)
            {
                Console.Write(b ? '#' : '.');
            }
            Console.WriteLine();
            while (gen < 20)
            {
                gen++;

                leftMost -= 2;
                current = NextGen(padding.Concat(current).Concat(padding).ToList(), rules).ToList();
                if (gen < 10)
                {
                    Console.Write(' ');
                }
                Console.Write(gen);
                Console.Write(": ");
                foreach (bool b in current) {
                    Console.Write(b ? '#' : '.');
                }
                Console.WriteLine();
            }
            int sum = 0;
            for (int i = 0; i < current.Count; ++i)
            {
                if (current[i])
                {
                    sum += (i + leftMost);
                }
            }
            Console.WriteLine($"{sum}");
            // guessed 4742, 8690
        }
    }
}
