﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace AdventDay1
{
    class Program
    {
        public static IEnumerable<TResult> Scan<TResult, T>(IEnumerable<T> source, TResult initial, Func<TResult, T, TResult> aggregator)
        {
            TResult currentResult = initial;
            foreach (T element in source)
            {
                currentResult = aggregator(currentResult, element);
                yield return currentResult;
            }
            yield break;
        }

        public static IEnumerable<T> RepeatForever<T>(IEnumerable<T> enumerable)
        {
            while (true)
            {
                foreach(T item in enumerable)
                {
                    yield return item;
                }
            }
        }

        static void Main(string[] args)
        {
            var lines = File.ReadLines("../../../input.txt");
            var numbers = lines.Select(line => int.Parse(line));
            var sum = numbers.Sum();
            Console.WriteLine(string.Format("Tune in to Frequency: {0}", sum));

            var repeatedNumbers = RepeatForever(numbers);

            var frequencies = Scan(RepeatForever(numbers),
                (freq:0, visited: ImmutableHashSet<int>.Empty, freqWasSeenBefore: false),
                (previous, delta) => {
                    var freq = previous.freq + delta;
                    return (
                        freq,
                        visited: previous.visited.Add(freq),
                        freqWasSeenBefore: previous.visited.Contains(freq)
                    );
                }
            );
            Console.WriteLine(string.Format("Tune in to Frequency: {0}", frequencies.First(res => res.freqWasSeenBefore).freq));
        }
    }
}
