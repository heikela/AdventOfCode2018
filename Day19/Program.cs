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
        public int[] Execute(int[] initialRegs)
        {
            int[] registers = initialRegs;
//            long cycle = 0;
            while (registers[IPReg] >= 0 && registers[IPReg] < Instructions.Count())
            {
                registers = Instructions[registers[IPReg]].Execute(registers);
                registers[IPReg]++;
/*                Console.WriteLine($"Cycle {cycle++}");
                foreach (int i in Enumerable.Range(0, registers.Count()))
                {
                    Console.WriteLine($"Register {i}: {registers[i]}");
                }*/
            }
            return registers;
        }
    }

    class MainProgram {
        static void Main(string[] args)
        {
            var input = File.ReadLines("../../../input.txt");
            var program = new Program(int.Parse(input.First().Substring(4,1)), input.Skip(1));
            var result = program.Execute(new int[6]);
            foreach (int i in Enumerable.Range(0, result.Count()))
            {
                Console.WriteLine($"Register {i}: {result[i]}");
            }
            result = program.Execute(Enumerable.Range(1,1).Concat(new int[5]).ToArray());
            foreach (int i in Enumerable.Range(0, result.Count()))
            {
                Console.WriteLine($"Register {i}: {result[i]}");
            }
        }
    }
}
