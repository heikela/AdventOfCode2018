using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Day15
{
    abstract class Creature
    {
        public int HP { get; set; }
        public Creature(int hp)
        {
            HP = hp;
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
        public Elf(int hp) : base(hp)
        {
        }
        public override bool IsElf()
        {
            return true;
        }
    }

    class Goblin : Creature
    {
        public Goblin(int hp) : base(hp)
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
        public static Tile FromChar(char inputChar)
        {
            switch (inputChar)
            {
                case '#':
                    return new Tile('#');
                case '.':
                    return new Tile('.');
                case 'G':
                    return new Tile('.', new Goblin(200));
                case 'E':
                    return new Tile('.', new Elf(200));
                default:
                    throw new InvalidOperationException("Unknown map entry");
            }
        }
        public bool HasCreature()
        {
            return Creature != null;
        }
    }

    class Game
    {
        private int W;
        private int H;
        Tile[,] Map;
        public Game(IEnumerable<string> lines)
        {
            H = lines.Count();
            W = lines.Select(l => l.Length).Max();
            Map = new Tile[W, H];
            int y = 0;
            foreach (string l in lines)
            {
                int x = 0;
                foreach (char c in l)
                {
                    Map[x, y] = Tile.FromChar(c);
                    if (Map[x,y].HasCreature())
                    {
                        Creature creature = Map[x, y].Creature;
                        if (creature.IsElf())
                        {

                        }
                    }
                    ++x;
                }
                ++y;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            new Battle battle = new Battle(File.ReadLines("../../../input.txt");
            while (!battle.IsOver())
            {
                battle.Step();
            }
            int outcome = battle.Outcome();
            Console.WriteLine($"Battle outcome: {outcome}");
        }
    }
}
