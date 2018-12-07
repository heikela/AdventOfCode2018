using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            string shiftPattern = ".*Guard #(\\d+) begins shift";
            string fallAsleepPattern = ".*:(\\d+)\\] falls asleep";
            string wakeupPattern = ".*:(\\d+)\\] wakes up";

            var lines = File.ReadLines("../../../input.txt");
            var chronological = lines.OrderBy(line => line);
            var sleepTimes = new Dictionary<int, int>();
            var guardSleepTimes = new Dictionary<int, int[]>();
            var currentGuard = -1;
            var asleepSince = -1;
            foreach (string line in chronological)
            {
                var shiftMatch = Regex.Match(line, shiftPattern);
                var fallAsleeptMatch = Regex.Match(line, fallAsleepPattern);
                var wakeupMatch = Regex.Match(line, wakeupPattern);
                if (shiftMatch.Success)
                {
                    currentGuard = int.Parse(shiftMatch.Groups[1].Value);
                    asleepSince = -1;
                }
                if (fallAsleeptMatch.Success)
                {
                    asleepSince = int.Parse(fallAsleeptMatch.Groups[1].Value);
                }
                if (wakeupMatch.Success)
                {
                    if (asleepSince == -1)
                    {
                        Console.WriteLine("Error processing line:");
                        Console.WriteLine(line);
                        Console.WriteLine("Waking up without falling asleep");
                    }
                    int wakeup = int.Parse(wakeupMatch.Groups[1].Value);
                    int slept = wakeup - asleepSince;
                    if (!sleepTimes.ContainsKey(currentGuard)) {
                        sleepTimes.Add(currentGuard, slept);
                        guardSleepTimes.Add(currentGuard, new int[60]);
                    } else
                    {
                        var alreadySlept = sleepTimes[currentGuard];
                        sleepTimes[currentGuard] = alreadySlept + slept;
                    }
                    for (int m = asleepSince; m < wakeup; ++m)
                    {
                        guardSleepTimes[currentGuard][m]++;
                    }
                }
            }
            var maxId = -1;
            var maxVal = 0;
            var maxMinute = -1;
            var maxMinuteValue = 0;
            foreach (KeyValuePair<int, int> sleepresult in sleepTimes)
            {
                if (sleepresult.Value > maxVal)
                {
                    maxId = sleepresult.Key;
                    maxVal = sleepresult.Value;

                    maxMinute = -1;
                    maxMinuteValue = 0;
                    for (int m = 0; m < 60; ++m)
                    {
                        var timesSleptOnThisMinute = guardSleepTimes[sleepresult.Key][m];
                        if (timesSleptOnThisMinute > maxMinuteValue)
                        {
                            maxMinute = m;
                            maxMinuteValue = timesSleptOnThisMinute;
                        }
                    }
                }
            }
            Console.WriteLine($"Guard who slept most is {maxId}, he slept {maxVal} minutes");
            Console.WriteLine($"He slepts most frequently on minute {maxMinute} - a total of {maxMinuteValue} times");
            Console.WriteLine($"ID * minute = {maxId * maxMinute}");

            maxId = -1;
            maxMinute = -1;
            maxMinuteValue = 0;
            foreach (int guard in guardSleepTimes.Keys)
            {
                for (int m = 0; m < 60; ++m)
                {
                    if (guardSleepTimes[guard][m] > maxMinuteValue)
                    {
                        maxId = guard;
                        maxMinute = m;
                        maxMinuteValue = guardSleepTimes[guard][m];
                    }
                }
            }
            Console.WriteLine($"Guard {maxId} is most frequently asleep on minute {maxMinute} - a total of {maxMinuteValue} times");
            Console.WriteLine($"ID * minute = {maxId * maxMinute}");

        }

    }
}
