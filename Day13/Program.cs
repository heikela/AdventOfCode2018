using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day13
{
    struct IntPoint2D : IEquatable<IntPoint2D>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public IntPoint2D(int x, int y)
        {
            X = x;
            Y = y;
        }
        public static IntPoint2D operator +(IntPoint2D a, IntPoint2D b)
        {
            return new IntPoint2D(a.X + b.X, a.Y + b.Y);
        }
        public static IntPoint2D operator -(IntPoint2D a, IntPoint2D b)
        {
            return new IntPoint2D(a.X - b.X, a.Y - b.Y);
        }

        public static IntPoint2D operator *(int m, IntPoint2D point)
        {
            return new IntPoint2D(point.X * m, point.Y * m);
        }

        public override bool Equals(object obj)
        {
            if (obj is IntPoint2D)
            {
                return this.Equals((IntPoint2D)obj);
            }
            return false;
        }
        public bool Equals(IntPoint2D p)
        {
            return X == p.X && Y == p.Y;
        }
        public static bool operator ==(IntPoint2D a, IntPoint2D b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(IntPoint2D a, IntPoint2D b)
        {
            return !(a.Equals(b));
        }
    }

    class Cart
    {
        public IntPoint2D Dir { get; set; }
        public int Crossroads { get; set; }
        private int LastStep;
        public Cart(IntPoint2D dir)
        {
            Dir = dir;
            Crossroads = 0;
            LastStep = 0;
        }
        public void TurnRight()
        {
            Dir = new IntPoint2D(-Dir.Y, Dir.X);
        }
        public void TurnLeft()
        {
            Dir = new IntPoint2D(Dir.Y, -Dir.X);
        }
        public bool Step(int time)
        {
            if (time <= LastStep)
            {
                return false;
            }
            ++LastStep;
            return true;
        }
    }

    class Simulation
    {
        private char[,] Grid;
        private Dictionary<IntPoint2D, Cart> Carts;
        private int Time;
        public Simulation(IEnumerable<string> lines)
        {
            int h = lines.Count() + 1;
            int w = lines.Select(l => l.Count()).Max() + 1;

            Grid = new char[w, h];
            Carts = new Dictionary<IntPoint2D, Cart>();
            Time = 0;

            {
                int y = 0;
                foreach (string line in lines)
                {
                    int x = 0;
                    foreach (char c in line)
                    {
                        switch (c)
                        {
                            case '<':
                                Grid[x, y] = '-';
                                Carts.Add(new IntPoint2D(x, y), new Cart(new IntPoint2D(-1, 0)));
                                break;
                            case '>':
                                Grid[x, y] = '-';
                                Carts.Add(new IntPoint2D(x, y), new Cart(new IntPoint2D(1, 0)));
                                break;
                            case 'v':
                                Grid[x, y] = '|';
                                Carts.Add(new IntPoint2D(x, y), new Cart(new IntPoint2D(0, 1)));
                                break;
                            case '^':
                                Grid[x, y] = '|';
                                Carts.Add(new IntPoint2D(x, y), new Cart(new IntPoint2D(0, -1)));
                                break;
                            default:
                                Grid[x, y] = c;
                                break;
                        }
                        ++x;
                    }
                    ++y;
                }
            }
        }

        private void Log(string message, IntPoint2D pos)
        {
            Console.WriteLine($"Step: {Time} Pos: {pos.X},{pos.Y}. {message}");
        }

        public void Step()
        {
            Time++;
            for (int y = 0; y < Grid.GetLength(1); ++y)
            {
                for (int x = 0; x < Grid.GetLength(0); ++x)
                {
                    IntPoint2D pos = new IntPoint2D(x, y);
                    if (Carts.ContainsKey(pos))
                    {
                        Cart cart = Carts[pos];
                        if (cart.Step(Time))
                        {
                            IntPoint2D dir = cart.Dir;
                            IntPoint2D nextPos = pos + dir;
                            if (Carts.ContainsKey(nextPos))
                            {
                                Carts.Remove(nextPos);
                                Carts.Remove(pos);
                                Log($"Collision! {Carts.Count} carts remain.", nextPos);
                            }
                            else
                            {
                                Carts.Remove(pos);
                                Carts.Add(nextPos, cart);
                            }
                            switch (Grid[nextPos.X, nextPos.Y])
                            {
                                case '/':
                                    if (dir.X != 0)
                                    {
                                        cart.TurnLeft();
                                    }
                                    else if (dir.Y != 0)
                                    {
                                        cart.TurnRight();
                                    }
                                    else
                                    {
                                        Log("Going off track!", nextPos);
                                    }
                                    break;
                                case '\\':
                                    if (dir.X != 0)
                                    {
                                        cart.TurnRight();
                                    }
                                    else if (dir.Y != 0)
                                    {
                                        cart.TurnLeft();
                                    }
                                    else
                                    {
                                        Log("Going off track!", nextPos);
                                    }
                                    break;
                                case '+':
                                    {
                                        int turnType = cart.Crossroads % 3;
                                        cart.Crossroads++;
                                        switch (turnType)
                                        {
                                            case 0:
                                                cart.TurnLeft();
                                                break;
                                            case 1:
                                                break;
                                            case 2:
                                                cart.TurnRight();
                                                break;
                                            default:
                                                Log("No fourth option at crossings!", nextPos);
                                                break;
                                        }
                                        break;
                                    }
                                case ' ':
                                    Log("Gone off track previously!", nextPos);
                                    break;
                                default: break;
                            }
                        }
                    }
                }
            }
            if (Carts.Count == 1)
            {
                Log("Final Cart", Carts.First().Key);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("../../../input.txt");
            Simulation s = new Simulation(lines);
            while (true)
            {
                s.Step();
            }
            Console.WriteLine("Hello World!");
        }
    }
}
