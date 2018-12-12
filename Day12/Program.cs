using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Day12
{
    class Program
    {
        struct GrowthPattern
        {
            public int LeftMost { get; private set; }
            public long Gen { get; private set; }
            public List<bool> Pattern { get; private set; }

            private static List<bool> Padding = new List<bool>() { false, false, false, false };


            public GrowthPattern(int left, long gen, List<bool> pattern)
            {
                LeftMost = left;
                Gen = gen;
                Pattern = pattern;
            }

            private IEnumerable<bool> NextGenEnumerable(Dictionary<(bool, bool, bool, bool, bool), bool> rules)
            {
                var padded = Padding.Concat(Pattern).Concat(Padding).ToList();
                for (int i = 2; i < padded.Count - 2; ++i)
                {
                    var key = (padded[i - 2], padded[i - 1], padded[i], padded[i + 1], padded[i + 2]);
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

            public GrowthPattern NextGen(Dictionary<(bool, bool, bool, bool, bool), bool> rules)
            {
                return new GrowthPattern(
                    LeftMost - 2,
                    Gen + 1,
                    NextGenEnumerable(rules).ToList());
            }

            public void Write()
            {
                if (Gen < 10)
                {
                    Console.Write(' ');
                }
                Console.Write(Gen);
                Console.Write(": ");
                foreach (bool b in Pattern)
                {
                    Console.Write(b ? '#' : '.');
                }
                Console.WriteLine();
            }

            public int Sum { get
                {
                    int sum = 0;
                    for (int i = 0; i < Pattern.Count; ++i)
                    {
                        if (Pattern[i])
                        {
                            sum += (i + LeftMost);
                        }
                    }
                    return sum;
                }
            }
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

            GrowthPattern current = new GrowthPattern(0, 0, initial);
            long prevSum = current.Sum;
            const long step50B = 50000000000;
            while (current.Gen < step50B)
            {
                current = current.NextGen(rules);
                long sum = current.Sum;
                long forecast = sum + (sum - prevSum) * (step50B - current.Gen);
                prevSum = sum;
                Console.WriteLine($"Gen: {current.Gen}, Sum: {sum}, forecast for 50B:th Gen: {forecast}");
            }
        }
    }
}
