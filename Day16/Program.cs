using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Day16
{
    struct Instruction
    {
        public int Opcode { get; set; }
        public int Input1 { get; set; }
        public int Input2 { get; set; }
        public int Result { get; set; }
        public Instruction(int opcode, int i1, int i2, int r)
        {
            Opcode = opcode;
            Input1 = i1;
            Input2 = i2;
            Result = r;
        }
        public static readonly string[] mnemonics = new string[] {
            "addr",
            "addi",
            "mulr",
            "muli",
            "banr",
            "bani",
            "borr",
            "bori",
            "setr",
            "seti",
            "gtir",
            "gtri",
            "gtrr",
            "eqir",
            "eqri",
            "eqrr"
        };
        public static int[] Execute(string mnemonic, int input1, int input2, int result_reg, int[] registers)
        {
            int[] result = (int[])registers.Clone();
            switch (mnemonic)
            {
                case "addr":
                    result[result_reg] = registers[input1] + registers[input2];
                    return result;
                case "addi":
                    result[result_reg] = registers[input1] + input2;
                    return result;
                case "mulr":
                    result[result_reg] = registers[input1] * registers[input2];
                    return result;
                case "muli":
                    result[result_reg] = registers[input1] * input2;
                    return result;
                case "banr":
                    result[result_reg] = registers[input1] & registers[input2];
                    return result;
                case "bani":
                    result[result_reg] = registers[input1] & input2;
                    return result;
                case "borr":
                    result[result_reg] = registers[input1] | registers[input2];
                    return result;
                case "bori":
                    result[result_reg] = registers[input1] | input2;
                    return result;
                case "setr":
                    result[result_reg] = registers[input1];
                    return result;
                case "seti":
                    result[result_reg] = input1;
                    return result;
                case "gtir":
                    result[result_reg] = input1 > registers[input2] ? 1 : 0;
                    return result;
                case "gtri":
                    result[result_reg] = registers[input1] > input2 ? 1 : 0;
                    return result;
                case "gtrr":
                    result[result_reg] = registers[input1] > registers[input2] ? 1 : 0;
                    return result;
                case "eqir":
                    result[result_reg] = input1 == registers[input2] ? 1 : 0;
                    return result;
                case "eqri":
                    result[result_reg] = registers[input1] == input2 ? 1 : 0;
                    return result;
                case "eqrr":
                    result[result_reg] = registers[input1] == registers[input2] ? 1 : 0;
                    return result;
                default:
                    throw new ArgumentException($"Unknown mnemonic: {mnemonic}");
            }
        }
    }
    class Sample
    {
        public int[] Before { get; set; }
        public int[] After { get; set; }
        public Instruction Instruction { get; set; }
        private Sample(int b0, int b1, int b2, int b3, int a0, int a1, int a2, int a3, int op, int i1, int i2, int r)
        {
            Before = new int[4] { b0, b1, b2, b3 };
            After = new int[4] { a0, a1, a2, a3 };
            Instruction = new Instruction(op, i1, i2, r);
        }
        public static Sample TryParse(IEnumerator<string> lineSource)
        {
            if (!lineSource.MoveNext())
            {
                return null;
            }
            var beforeMatch = Regex.Match(lineSource.Current, "Before: \\[(\\d+), (\\d+), (\\d+), (\\d+)\\]");
            if (!beforeMatch.Success)
            {
                return null;
            }
            if (!lineSource.MoveNext())
            {
                return null;
            }
            var instructionMatch = Regex.Match(lineSource.Current, "(\\d+) +(\\d+) +(\\d+) +(\\d+)");
            if (!instructionMatch.Success)
            {
                return null;
            }
            if (!lineSource.MoveNext())
            {
                return null;
            }
            var afterMatch = Regex.Match(lineSource.Current, "After: +\\[(\\d+), *(\\d+), *(\\d+), *(\\d+)\\]");
            if (!afterMatch.Success)
            {
                return null;
            }
            lineSource.MoveNext();
            return new Sample(
                int.Parse(beforeMatch.Groups[1].Value),
                int.Parse(beforeMatch.Groups[2].Value),
                int.Parse(beforeMatch.Groups[3].Value),
                int.Parse(beforeMatch.Groups[4].Value),
                int.Parse(afterMatch.Groups[1].Value),
                int.Parse(afterMatch.Groups[2].Value),
                int.Parse(afterMatch.Groups[3].Value),
                int.Parse(afterMatch.Groups[4].Value),
                int.Parse(instructionMatch.Groups[1].Value),
                int.Parse(instructionMatch.Groups[2].Value),
                int.Parse(instructionMatch.Groups[3].Value),
                int.Parse(instructionMatch.Groups[4].Value)
            );
        }
        public bool MatchMnemonic(string m)
        {
            int[] res = Instruction.Execute(m, Instruction.Input1, Instruction.Input2, Instruction.Result, Before);
            for (int i = 0; i < 4; ++i)
            {
                if (res[i] != After[i])
                {
                    return false;
                }
            }
            return true;
        }
        public IEnumerable<string> MatchingMnemonics()
        {
            return Instruction.mnemonics.Where(m => MatchMnemonic(m));
        }
    }
    class Program
    {
        static IEnumerable<Sample> ParseSamples(IEnumerable<string> lines)
        {
            IEnumerator<string> source = lines.GetEnumerator();
            Sample sample;
            while (true)
            {
                sample = Sample.TryParse(source);
                if (sample != null)
                {
                    yield return (Sample)sample;
                }
                else
                {
                    yield break;
                }
            }
        }
        static void Main(string[] args)
        {
            var input = File.ReadLines("../../../input.txt");
            var samples = ParseSamples(input).ToList();
            IEnumerable<Sample> matchingThreeOrMore = samples.Where(s => s.MatchingMnemonics().Count() >= 3);
            Console.WriteLine($"{matchingThreeOrMore.Count()} samples match three or more instructions.");
        }
    }
}
