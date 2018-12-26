using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Day13
{
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
        private int W;
        public Simulation(IEnumerable<string> lines)
        {
            int h = lines.Count() + 1;
            W = lines.Select(l => l.Count()).Max() + 1;

            Grid = new char[W, h];
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

        public int CartCount()
        {
            return Carts.Count;
        }

        private void Log(string message, IntPoint2D pos)
        {
            Console.WriteLine($"Step: {Time} Pos: {pos.X},{pos.Y}. {message}");
        }

        public void Step()
        {
            Time++;
            foreach (IntPoint2D pos in Carts.Keys.ToList().OrderBy(pos => pos.Y * W + pos.X)) {
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
            while (s.CartCount() > 1)
            {
                s.Step();
            }
        }
    }
}
