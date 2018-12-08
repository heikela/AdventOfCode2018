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

            string answer = null;
            string[] lineArray = lines.ToArray();
            for (int i = 0; answer == null && i < lineArray.Count(); ++i)
            {
                for (int j = i + 1; j < lineArray.Count(); ++j)
                {
                    var errors = 0;
                    var matching = "";
                    for (int c = 0; c < lineArray[i].Length; ++c)
                    {
                        if (lineArray[i][c] == lineArray[j][c])
                        {
                            matching += lineArray[i][c];
                        }
                        else
                        {
                            ++errors;
                        }
                    }
                    if (errors == 1)
                    {
                        answer = matching;
                        break;
                    }
                }
            }
            System.Console.WriteLine($"Common code = {answer}");

        }
    }
}
