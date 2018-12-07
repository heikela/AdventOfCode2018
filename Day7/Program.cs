using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day7
{
    class Program
    {
        static void Main(string[] args)
        {
            //var lines = File.ReadLines("../../../sampleinput.txt");
            var lines = File.ReadLines("../../../input.txt");
            var adjacency = lines.Select(l => (first: l[5], second: l[36]));
            var preqs = new Dictionary<char, HashSet<char>>();
            var done = new HashSet<char>();
            var remaining = new HashSet<char>(adjacency.SelectMany(a => new List<char>() { a.first, a.second}).Distinct());
            foreach (var g in adjacency.GroupBy(a => a.second))
            {
                preqs[g.Key] = new HashSet<char>(g.Select(a => a.first));
            }
            var workerFinishingTimes = new int[5];
            var time = 0;
            var running = new SortedDictionary<char, int>();
            while (remaining.Count > 0)
            {
                var available = remaining.Where(step =>
                {
                    if (!preqs.ContainsKey(step))
                    {
                        return true;
                    }
                    foreach (var preq in preqs[step])
                    {
                        if (!done.Contains(preq))
                        {
                            return false;
                        }
                    }
                    return true;
                });
                while (available.Count() > 0 && workerFinishingTimes.Min() <= time)
                {
                    var chosen = available.OrderBy(c => c).First();
                    Console.Write("" + chosen);
                    remaining.Remove(chosen);
                    var worker = -1;
                    for (int i = 0; i < 5; ++i)
                    {
                        if (workerFinishingTimes[i] <= time)
                        {
                            worker = i;
                            break;
                        }
                    }
                    var finishingTime = time + 61 + chosen - 'A';
                    workerFinishingTimes[worker] = finishingTime;
                    Console.WriteLine($"Worker {worker} starting task {chosen} at time {time}. It will finish at {finishingTime}");
                    running.Add(chosen, finishingTime);
                    available = available.Where(step => step != chosen);
                }

/*                if (available.Count() > 0)
                {*/
                    time = running.Select(item => item.Value).Min();
                    foreach (var step in running.Keys.ToList())
                    {
                        if (running[step] <= time)
                        {
                            running.Remove(step);
                            done.Add(step);
                        }
                    }
/*
                }

                var minFinishTime = workerFinishingTimes.Min();
                if (minFinishTime > time)
                {
                    time = minFinishTime;
                    foreach (var step in running.Keys.ToList())
                    {
                        if (running[step] <= time)
                        {
                            running.Remove(step);
                        }
                    }
                }*/
            }
            Console.WriteLine($"Part two, finishing time in parallel task running is: {workerFinishingTimes.Max()}");
        }
    }
}
