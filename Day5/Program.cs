using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day5
{
    class Program
    {
        static string Collapsed(string input)
        {
            return input.Aggregate("", (prefix, current) =>
            {
                if (prefix.Count() == 0)
                {
                    return prefix + current;
                }
                if (char.ToLower(prefix.Last()) != char.ToLower(current))
                {
                    return prefix + current;
                }
                if (char.IsLower(prefix.Last()) && char.IsLower(current))
                {
                    return prefix + current;
                }
                if (char.IsUpper(prefix.Last()) && char.IsUpper(current))
                {
                    return prefix + current;
                }
                return prefix.Substring(0, prefix.Length - 1);
            });
        }

        static int CollapsedLen(IEnumerable<char> input)
        {
            Stack<char> reversePrefix = new Stack<char>();
            foreach (char c in input)
            {
                if (reversePrefix.Count == 0)
                {
                    reversePrefix.Push(c);
                }
                else
                {
                    char top = reversePrefix.Peek();
                    if (char.ToLower(top) != char.ToLower(c)
                        || char.IsLower(top) && char.IsLower(c)
                        || char.IsUpper(top) && char.IsUpper(c))
                    {
                        reversePrefix.Push(c);
                    }
                    else
                    {
                        reversePrefix.Pop();
                    }
                }
            };
            return reversePrefix.Count;
        }

        static void Main(string[] args)
        {
            var lines = File.ReadLines("../../../input.txt");
            var polymer = lines.Aggregate((a, b) => a + b);

/*            var collapsed = Collapsed(polymer);
            Console.WriteLine($"Length of resulting polymer is {collapsed.Count()}");
*/
            Console.WriteLine($"Length of resulting polymer is {CollapsedLen(polymer)}");

            HashSet<char> letters = new HashSet<char>();
            foreach (char c in polymer.Select(c => char.ToLower(c)).Distinct())
            {
                letters.Add(c);
            }

            int minLen = polymer.Length;
            char minChar = '?';

            foreach (char c in letters)
            {
                //                var shorter = Collapsed(polymer.Where(cc => char.ToLower(cc) != c).Aggregate<char, string>("", (p, cc) => p + cc));
                var shorter = CollapsedLen(polymer.Where(cc => char.ToLower(cc) != c));
//                if (shorter.Length < minLen)
                if (shorter < minLen)
                    {
                    //                        minLen = shorter.Length;
                    minLen = shorter;
                    minChar = c;
                }
                Console.WriteLine($"Checked {c}. So far the best char to remove is {minChar}, resulting in len = {minLen}");
            }
            Console.WriteLine($"Best char to remove is {minChar}, resulting in len = {minLen}");
        }
    }
}
