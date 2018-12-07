using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace AdventDay1
{
    class Program
    {
        public static IEnumerable<TResult> Aggregated<TResult, T>(IEnumerable<T> source, TResult initial, Func<TResult, T, TResult> aggregator)
        {
            TResult currentResult = initial;
            foreach (T element in source)
            {
                currentResult = aggregator(currentResult, element);
                yield return currentResult;
            }
            yield break;
        }

        public class AggregateEnumerator<TResult, T> : IEnumerator<TResult>
        {
            private Func<TResult, T, TResult> Aggregator;
            private TResult CurrentResult;
            private TResult Initial;
            private IEnumerator<T> Source;

            public AggregateEnumerator(IEnumerator<T> source, TResult initial, Func<TResult, T, TResult> aggregator)
            {
                CurrentResult = Initial = initial;
                Source = source;
                Aggregator = aggregator;
            }
            public bool MoveNext()
            {
                bool unfinished = Source.MoveNext();
                if (unfinished)
                {
                    CurrentResult = Aggregator(CurrentResult, Source.Current);
                }
                return unfinished;
            }

            public void Reset()
            {
                CurrentResult = Initial;
                Source.Reset();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            TResult IEnumerator<TResult>.Current => CurrentResult;
            Object IEnumerator.Current
            {
                get { return CurrentResult; }
            }
        }

        static void Main(string[] args)
        {
            var lines = File.ReadLines("../../../input.txt");
            var numbers = lines.Select(line => int.Parse(line)).ToList();
            var sum = numbers.Sum();
            Console.WriteLine(string.Format("Tune in to Frequency: {0}", sum));

            var visited = new HashSet<int>();

            bool found = false;
            int currentFreq = 0;
            while (!found)
            {
                var changes = numbers.GetEnumerator();
                while (!found && changes.MoveNext())
                {
                    currentFreq += changes.Current;
                    if (visited.Contains(currentFreq))
                    {
                        found = true;
                    }
                    visited.Add(currentFreq);
                }
            }
            Console.WriteLine(string.Format("Tune in to Frequency: {0}", currentFreq));

            var repeatedNumbers = Enumerable.Repeat(numbers, 1000).SelectMany(List => List);

            var frequencies = Aggregated(repeatedNumbers, (0, new HashSet<int>(), false),
                (result, number) => {
                    var freq = result.Item1 + number;
                    var set = result.Item2;
                    bool repeated = set.Contains(freq);
                    set.Add(freq);
                    return (
                        freq,
                        set,
                        repeated
                    );
                }
            );
            Console.WriteLine(string.Format("Tune in to Frequency: {0}", frequencies.First(res => res.Item3).Item1));

        }
    }
}
