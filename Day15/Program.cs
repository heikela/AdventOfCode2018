﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Common;

namespace Day15
{
    abstract class Creature
    {
        public int HP { get; set; }
        public IntPoint2D Pos { get; set; }
        public Creature(int hp, int x, int y)
        {
            HP = hp;
            Pos = new IntPoint2D(x, y);
        }
        public virtual bool IsElf()
        {
            return false;
        }
        public virtual bool IsGoblin()
        {
            return false;
        }
    }

    class Elf : Creature
    {
        public Elf(int hp, int x, int y) : base(hp, x, y)
        {
        }
        public override bool IsElf()
        {
            return true;
        }
    }

    class Goblin : Creature
    {
        public Goblin(int hp, int x, int y) : base(hp, x, y)
        {
        }
        public override bool IsGoblin()
        {
            return true;
        }
    }
    struct Tile
    {
        public char Terrain { get; set; }
        public Creature Creature { get; set; }
        public Tile(char terrain, Creature creature)
        {
            Terrain = terrain;
            Creature = creature;
        }
        public Tile(char terrain)
        {
            Terrain = terrain;
            Creature = null;
        }
        public static Tile FromInput(char inputChar, int x, int y)
        {
            switch (inputChar)
            {
                case '#':
                    return new Tile('#');
                case '.':
                    return new Tile('.');
                case 'G':
                    return new Tile('.', new Goblin(200, x, y));
                case 'E':
                    return new Tile('.', new Elf(200, x, y));
                default:
                    throw new InvalidOperationException("Unknown map entry");
            }
        }
        public bool HasCreature()
        {
            return Creature != null;
        }
    }

    class Battle
    {
        private int W;
        private int H;
        private Tile[,] Map;
        private List<Creature> Elves;
        private List<Creature> Goblins;
        private int Turn;
        private bool LastActionWasKill;
        public bool ElvesKilled { get; private set; }
        private int ElfPower;
        private static readonly List<IntPoint2D> Adjacent = new List<IntPoint2D>() {
            new IntPoint2D(0, -1),
            new IntPoint2D(-1, 0),
            new IntPoint2D(1, 0),
            new IntPoint2D(0, 1)
        };
        public Battle(IEnumerable<string> lines, int elfPower = 3)
        {
            H = lines.Count();
            W = lines.Select(l => l.Length).Max();
            Map = new Tile[W, H];
            Turn = 0;
            int y = 0;
            Elves = new List<Creature>();
            Goblins = new List<Creature>();
            LastActionWasKill = false;
            ElfPower = elfPower;

            foreach (string l in lines)
            {
                int x = 0;
                foreach (char c in l)
                {
                    Map[x, y] = Tile.FromInput(c, x, y);
                    if (Map[x,y].HasCreature())
                    {
                        Creature creature = Map[x, y].Creature;
                        if (creature.IsElf())
                        {
                            Elves.Add(creature);
                        }
                        else if (creature.IsGoblin())
                        {
                            Goblins.Add(creature);
                        }
                    }
                    ++x;
                }
                ++y;
            }
        }

        public bool IsOver()
        {
            return Elves.Count == 0 || Goblins.Count == 0;
        }

        public int Outcome()
        {
            if (!IsOver())
            {
                return -1;
            } else
            {
                int duration = LastActionWasKill ? Turn : Turn - 1;
                return duration * Math.Max(Elves.Select(e => e.HP).Sum(), Goblins.Select(e => e.HP).Sum());
            }
        }

        private bool Occupied(IntPoint2D p)
        {
            return Map[p.X, p.Y].Terrain == '#' || Map[p.X, p.Y].HasCreature();
        }

        public void Step()
        {
            const int DAMAGE = 3;
            IEnumerable<Creature> creaturesInOrder = Elves.Concat(Goblins)
                .OrderBy(c => c.Pos.Y * W + c.Pos.X);
            foreach (Creature c in creaturesInOrder)
            {
                if (c.HP > 0)
                {
                    LastActionWasKill = false;
//                    Console.WriteLine($"Choosing a move for a/an {(c.IsElf() ? "elf" : "goblin")} at {c.Pos.X},{c.Pos.Y}.");
                    IEnumerable<Creature> enemies = c.IsElf() ? Goblins : Elves;
                    IEnumerable<Creature> adjacentEnemies = enemies.Where(e => e.Pos.ManhattanDist(c.Pos) == 1);
                    if (adjacentEnemies.Count() == 0)
                    {
                        IEnumerable<IntPoint2D> adjacentToEnemy = enemies.SelectMany(e => Adjacent.Select(step => e.Pos + step)).Distinct();
                        IEnumerable<IntPoint2D> adjacentAndFree = adjacentToEnemy.Where(p => !Occupied(p));
                        IEnumerable<IntPoint2D> path = PathToNearest(c.Pos, adjacentAndFree);
                        if (path.Count() > 0) {
//                            Console.WriteLine("Move");
                            Map[c.Pos.X, c.Pos.Y].Creature = null;
                            c.Pos = path.First();
                            Map[c.Pos.X, c.Pos.Y].Creature = c;
                        } else
                        {
//                            Console.WriteLine("Pass");
                        }
                    }
                    adjacentEnemies = enemies.Where(e => e.Pos.ManhattanDist(c.Pos) == 1);
                    if (adjacentEnemies.Count() > 0)
                    {
                        int minHP = int.MaxValue;
                        Creature target = null;
                        foreach (Creature e in adjacentEnemies)
                        {
                            if (e.HP < minHP)
                            {
                                target = e;
                                minHP = e.HP;
                            }
                            else if (e.HP == minHP)
                            {
                                if (e.Pos.Y < target.Pos.Y || (e.Pos.Y == target.Pos.Y && e.Pos.X < target.Pos.X))
                                {
                                    target = e;
                                }
                            }
                        }
                        int damage = c.IsElf() ? ElfPower : DAMAGE;
                        target.HP -= damage;
 //                       Console.WriteLine("Attack");
                        if (target.HP <= 0)
                        {
   //                         Console.WriteLine("Kill");
                            if (target.IsElf())
                            {
                                Elves.Remove(target);
                                ElvesKilled = true;
                            }
                            else
                            {
                                Goblins.Remove(target);
                            }
                            Map[target.Pos.X, target.Pos.Y].Creature = null;
                            LastActionWasKill = true;
                        }
                    }
                    else
                    {
//                        Console.WriteLine("Pass");
                    }
                }
            }
            Turn++;
//            Console.WriteLine($"End of time step {Turn}.");
        }

        public void PrintMap()
        {
            for (int y = 0; y < H; ++y)
            {
                Queue<Creature> creatures = new Queue<Creature>();
                for (int x = 0; x < W; ++x)
                {
                    if (Map[x,y].Creature != null && Map[x,y].Creature.IsElf())
                    {
                        Console.Write('E');
                        creatures.Enqueue(Map[x, y].Creature);
                    }
                    else if (Map[x, y].Creature != null && Map[x, y].Creature.IsGoblin())
                    {
                        Console.Write('G');
                        creatures.Enqueue(Map[x, y].Creature);
                    }
                    else
                    {
                        Console.Write(Map[x, y].Terrain);
                    }
                }
                while (creatures.Count > 0)
                {
                    Creature c = creatures.Dequeue();
                    Console.Write($" {(c.IsElf() ? "E" : "G")}({c.HP})");
                }
                Console.WriteLine();
            }
        }

        private IEnumerable<IntPoint2D> PathToNearest(IntPoint2D pos, IEnumerable<IntPoint2D> targets)
        {
            IEnumerable<IntPoint2D> poss = new List<IntPoint2D>() { pos };
            HashSet<IntPoint2D> visited = new HashSet<IntPoint2D>(poss);
            Dictionary<IntPoint2D, int> stepsNeeded = new Dictionary<IntPoint2D, int>();
            stepsNeeded.Add(pos, 0);
            int stepCount = 0;
            while (visited.Intersect(targets).Count() == 0 && poss.Count() > 0)
            {
                stepCount++;
                var allSteps = poss
                    .SelectMany(p => Adjacent.Select(move => p + move));
                var newSteps = allSteps.Where(p => !Occupied(p) && !visited.Contains(p));
                var distinctSteps = newSteps.Distinct();
                poss = distinctSteps.ToList();
                visited.UnionWith(poss);
                foreach (IntPoint2D p in poss)
                {
                    stepsNeeded.Add(p, stepCount);
                }
            }
            if (visited.Intersect(targets).Count() == 0)
            {
                return new List<IntPoint2D>();
            }
            else
            {
                IntPoint2D chosenTarget = visited.Intersect(targets).OrderBy(p => p.Y * W + p.X).First();
                Stack<IntPoint2D> reversePath = new Stack<IntPoint2D>();
                reversePath.Push(chosenTarget);
                while (reversePath.Peek() != pos)
                {
                    IntPoint2D current = reversePath.Peek();
                    IntPoint2D previousStep = Adjacent.Select(step => current + step)
                        .Where(p => stepsNeeded.GetValueOrDefault(p, -1) == stepsNeeded[current] - 1).First();
                    reversePath.Push(previousStep);
                }
                reversePath.Pop();
                return reversePath.ToList();
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Battle battle = new Battle(File.ReadLines("../../../sampleInput1.txt"));
            while (!battle.IsOver())
            {
                battle.Step();
//                battle.PrintMap();
            }
            int outcome = battle.Outcome();
            Console.WriteLine($"Sample battle outcome: {outcome}");
            Debug.Assert(outcome == 27730);

            battle = new Battle(File.ReadLines("../../../input.txt"));
            while (!battle.IsOver())
            {
                battle.Step();
            }
            outcome = battle.Outcome();
            Console.WriteLine($"Battle outcome: {outcome}");
            int elfPower = 4;
            while (true)
            {
                Console.WriteLine($"Trying battle with elf power: {elfPower}");
                battle = new Battle(File.ReadLines("../../../input.txt"), elfPower);
                while (!battle.IsOver() && !battle.ElvesKilled)
                {
                    battle.Step();
                }
                if (battle.ElvesKilled)
                {
                    ++elfPower;
                } else
                {
                    break;
                }
            }
            Console.WriteLine($"Outcome when no elves die: {battle.Outcome()}");
        }
    }
}
