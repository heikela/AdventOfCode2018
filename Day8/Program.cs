using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Day8
{
    class Node
    {
        public List<Node> Children { get; private set; }
        public List<int> Metadata { get; private set; }
        public Node(IEnumerator<int> source)
        {
            source.MoveNext();
            var numChildren = source.Current;
            source.MoveNext();
            var numMetadata = source.Current;
            Children = new List<Node>();
            Metadata = new List<int>();
            for (int i = 0; i < numChildren; ++i)
            {
                Children.Add(new Node(source));
            }
            for (int i = 0; i < numMetadata; ++i)
            {
                source.MoveNext();
                var metaDataItem = source.Current;
                Metadata.Add(metaDataItem);
            }
        }
        public int MetaDataSum()
        {
            return Children.Select(c => c.MetaDataSum()).Sum() + Metadata.Sum();
        }

        public int NodeValue()
        {
            int childCount = Children.Count();
            if (childCount == 0)
            {
                return Metadata.Sum();
            }
            return Metadata
                .Where(i => i <= childCount)
                .Select(i => Children[i - 1].NodeValue())
                .Sum();
        }
    };

    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("../../../input.txt");
            var numbers = lines.SelectMany(line => line.Split(' ')).Select(num => int.Parse(num));

            var root = new Node(numbers.GetEnumerator());

            Console.WriteLine($"The sum of the metadata entries is {root.MetaDataSum()}");
            Console.WriteLine($"The value of root is {root.NodeValue()}");
        }
    }
}
