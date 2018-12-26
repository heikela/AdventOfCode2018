using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("../../../input.txt");
            var letterGroupLengths = lines
                .Select(line => line.GroupBy(c => c))
                .Select(groups => groups.Select(group => group.Count()));

            int doubleCount = letterGroupLengths.Count(groups => groups.Any(length => length == 2));
            int tripleCount = letterGroupLengths.Count(groups => groups.Any(length => length == 3));
            System.Console.WriteLine($"Checksum = {doubleCount * tripleCount}");

            string answer = null;
            string[] lineArray = lines.ToArray();
            for (int i = 0; answer == null && i < lineArray.Count(); ++i)
            {
                for (int j = i + 1; j < lineArray.Count(); ++j)
                {
                    string candidate = string
                        .Join<char>("",
                            lineArray[i].Zip(lineArray[j],
                            (a, b) => a == b ? new char[] { a } : new char[0]).SelectMany(chars => chars));
                    if (candidate.Length == lineArray[i].Length - 1)
                    {
                        answer = candidate;
                        break;
                    }
                }
            }
            System.Console.WriteLine($"Common code = {answer}");
        }
    }
}
