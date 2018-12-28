using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day5
{
    class Program
    {
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

            Console.WriteLine($"Length of resulting polymer is {CollapsedLen(polymer)}");

            HashSet<char> letters = new HashSet<char>(polymer.Select(c => char.ToLower(c)));

            int minLen = polymer.Length;
            char minChar = '?';

            foreach (char c in letters)
            {
                var collapsedLen = CollapsedLen(polymer.Where(cc => char.ToLower(cc) != c));
                if (collapsedLen < minLen)
                    {
                    minLen = collapsedLen;
                    minChar = c;
                }
            }
            Console.WriteLine($"Best char to remove is {minChar}, resulting in len = {minLen}");
        }
    }
}
