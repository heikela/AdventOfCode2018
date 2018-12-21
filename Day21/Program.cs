using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Day19
{
    struct Instruction
    {
        public string Mnemonic { get; set; }
        public int Input1 { get; set; }
        public int Input2 { get; set; }
        public int Result { get; set; }
        public Instruction(string mnemonic, int i1, int i2, int r)
        {
            Mnemonic = mnemonic;
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
        public int[] Execute(int[] registers)
        {
            int[] result = (int[])registers.Clone();
            switch (Mnemonic)
            {
                case "addr":
                    result[Result] = registers[Input1] + registers[Input2];
                    return result;
                case "addi":
                    result[Result] = registers[Input1] + Input2;
                    return result;
                case "mulr":
                    result[Result] = registers[Input1] * registers[Input2];
                    return result;
                case "muli":
                    result[Result] = registers[Input1] * Input2;
                    return result;
                case "banr":
                    result[Result] = registers[Input1] & registers[Input2];
                    return result;
                case "bani":
                    result[Result] = registers[Input1] & Input2;
                    return result;
                case "borr":
                    result[Result] = registers[Input1] | registers[Input2];
                    return result;
                case "bori":
                    result[Result] = registers[Input1] | Input2;
                    return result;
                case "setr":
                    result[Result] = registers[Input1];
                    return result;
                case "seti":
                    result[Result] = Input1;
                    return result;
                case "gtir":
                    result[Result] = Input1 > registers[Input2] ? 1 : 0;
                    return result;
                case "gtri":
                    result[Result] = registers[Input1] > Input2 ? 1 : 0;
                    return result;
                case "gtrr":
                    result[Result] = registers[Input1] > registers[Input2] ? 1 : 0;
                    return result;
                case "eqir":
                    result[Result] = Input1 == registers[Input2] ? 1 : 0;
                    return result;
                case "eqri":
                    result[Result] = registers[Input1] == Input2 ? 1 : 0;
                    return result;
                case "eqrr":
                    result[Result] = registers[Input1] == registers[Input2] ? 1 : 0;
                    return result;
                default:
                    throw new ArgumentException($"Unknown mnemonic: {Mnemonic}");
            }
        }
        public static Instruction Parse(string line)
        {
            var instructionMatch = Regex.Match(line, "(\\w+) +(\\d+) +(\\d+) +(\\d+)");
            if (!instructionMatch.Success)
            {
                throw new ArgumentException();
            }
            return new Instruction(
                instructionMatch.Groups[1].Value,
                int.Parse(instructionMatch.Groups[2].Value),
                int.Parse(instructionMatch.Groups[3].Value),
                int.Parse(instructionMatch.Groups[4].Value)
            );
        }
    }

    class Program
    {
        private Instruction[] Instructions;
        private int IPReg;
        public Program(int IPReg, IEnumerable<string> lines)
        {
            this.IPReg = IPReg;
            Instructions = lines.Select(l => Instruction.Parse(l)).ToArray();
        }
        public (int[] state, int cycle) Execute(int[] initialRegs, int stopAtCycle = 100000, int initialCycle = 0)
        {
            int[] registers = initialRegs;
            int cycle = initialCycle;
            while (registers[IPReg] >= 0 && registers[IPReg] < Instructions.Count() && cycle < stopAtCycle)
            {
                registers = Instructions[registers[IPReg]].Execute(registers);
                registers[IPReg]++;
                cycle++;
                /*                Console.WriteLine($"Cycle {cycle++}");
                                foreach (int i in Enumerable.Range(0, registers.Count()))
                                {
                                    Console.WriteLine($"Register {i}: {registers[i]}");
                                }*/
            }
            return (registers, cycle);
        }
    }

    class MainProgram
    {
        static void Main(string[] args)
        {
            var input = File.ReadLines("../../../input.txt");
            var program = new Program(int.Parse(input.First().Substring(4, 1)), input.Skip(1));
            int haltingTime = int.MaxValue - 1;
            int startingValue = 0;
            const int step = 1;
            Dictionary<int, (int[] state, int cycle)> activeSearches = new Dictionary<int, (int[] state, int cycle)>();
            while (true)
            {
                activeSearches.Add(startingValue, (Enumerable.Range(startingValue, 1).Concat(new int[5]).ToArray(), 0));
                startingValue++;
                foreach (int search in activeSearches.Keys.ToList())
                {
                    var state = activeSearches[search].state;
                    int cycle = activeSearches[search].cycle;
                    var newState = program.Execute(state, Math.Min(cycle + step, haltingTime + 1), cycle);
                    activeSearches[search] = newState;
                    if (newState.cycle < cycle + step)
                    {
                        haltingTime = newState.cycle;
                        Console.WriteLine($"Program started with R0 = {search} halted after {newState.cycle} steps!");
                    }
                    if (newState.cycle > haltingTime)
                    {
                        activeSearches.Remove(search);
                        Console.WriteLine($"Program started with R0 = {search} runs longer than another program.");
                    }
                    if (Enumerable.Range(0, state.Length).All(i => state[i] == newState.state[i]))
                    {
                        // loop within one step detected;
                        activeSearches.Remove(search);
                        Console.WriteLine($"Program started with R0 = {search} enters a one instruction loop after {newState.cycle} steps.");
                    }
                }
            }
        }
    }
}
