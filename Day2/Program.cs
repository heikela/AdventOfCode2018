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
            var letterGroups = lines.Select(line => line.GroupBy(c => c));
            var letterGroupLengths = letterGroups
                .Select(groupsForLine =>
                    groupsForLine.Select<IGrouping<char, char>, int>(group => group.Count()));

            int doubleCount = 0;
            int tripleCount = 0;
            foreach (string line in lines)
            {
                var charGroups = line.GroupBy(c => c);
                if (charGroups.Count(group => group.Count() == 2) > 0)
                {
                    ++doubleCount;
                }
                if (charGroups.Count(group => group.Count() == 3) > 0)
                {
                    ++tripleCount;
                }
            }
            System.Console.WriteLine($"Checksum = {doubleCount * tripleCount}");

            var sorted = lines.OrderBy(s => s);
            string answer = null;
            sorted.Aggregate((prev, current) =>
            {
                var errors = 0;
                var matching = "";
                for (int i = 0; i < current.Length; ++i)
                {
                    if (current[i] == prev[i])
                    {
                        matching += current[i];
                    } else
                    {
                        ++errors;
                    }
                }
                if (errors == 1)
                {
                    answer = matching;
                }
                return current;
            });

            System.Console.WriteLine($"Common code = {answer}");

        }
    }
}
