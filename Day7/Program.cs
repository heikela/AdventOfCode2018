using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day7
{
    class Program
    {
        static IEnumerable<(char job, int completesAt)> Simulate(
            IEnumerable<(char prerequisite, char dependent)> adjacency,
            int parallel = 1)
        {
            var preqs = new Dictionary<char, HashSet<char>>(
                adjacency.GroupBy(a => a.dependent)
                    .Select(group => new KeyValuePair<char, HashSet<char>>(group.Key, new HashSet<char>(group.Select(a => a.prerequisite)))));

            var done = new HashSet<char>();
            var remaining = new HashSet<char>(adjacency.SelectMany(a => new char[] { a.prerequisite, a.dependent }).Distinct());
            var workerStates = new (int idx, int finishingTime, char task)[parallel];
            for (int i = 0; i < parallel; ++i)
            {
                workerStates[i].idx = i;
            }

            var time = 0;
            while (remaining.Count > 0)
            {
                IEnumerable<char> availableTasks = remaining.Where(task => !preqs.ContainsKey(task) || preqs[task].All(preq => done.Contains(preq)));
                Stack<int> availableWorkers = new Stack<int>(
                    workerStates
                        .Where(state => state.finishingTime <= time)
                        .Select(state => state.idx));
                while (availableTasks.Count() > 0 && availableWorkers.Count > 0)
                {
                    var chosenTask = availableTasks.OrderBy(c => c).First();
                    remaining.Remove(chosenTask);
                    availableTasks = availableTasks.Where(task => task != chosenTask);
                    var chosenWorker = availableWorkers.Pop();
                    int finishingTime = time + 61 + chosenTask - 'A';
                    workerStates[chosenWorker].task = chosenTask;
                    workerStates[chosenWorker].finishingTime = finishingTime;
                    yield return (job: chosenTask, completesAt: finishingTime);
                }
                int newTime = workerStates.Where(state => state.finishingTime > time).Select(state => state.finishingTime).Min();
                foreach (var workerState in workerStates)
                {
                    if (workerState.finishingTime > time && workerState.finishingTime <= newTime)
                    {
                        done.Add(workerState.task);
                    }
                }
                time = newTime;
            }
            yield break;
        }
        static void Main(string[] args)
        {
            var lines = File.ReadLines("../../../input.txt");
            var adjacency = lines.Select(l => (prerequisite: l[5], dependent: l[36]));
            var sequentialSequence = Simulate(adjacency).Select(completion => completion.job).Aggregate("", (a, b) => a + b);
            Console.WriteLine($"Tasks can be done in order {sequentialSequence}");
            var parallelDuration = Simulate(adjacency, 5).Select(completion => completion.completesAt).Max();
            Console.WriteLine($"Part two, finishing time in parallel task running is: {parallelDuration}");
        }
    }
}
