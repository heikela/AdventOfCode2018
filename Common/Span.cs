using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public struct IntSpan
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public IntSpan(int min, int max)
        {
            Min = min;
            Max = max;
        }
        public static IntSpan Inclusive(int a, int b)
        {
            if (a < b)
            {
                return new IntSpan(a, b);
            }
            return new IntSpan(b, a);
        }
        public static IntSpan HalfOpen(int a, int b)
        {
            if (a < b)
            {
                return new IntSpan(a, b - 1);
            }
            return new IntSpan(b, a - 1);
        }
        public static IntSpan Inclusive(double a, double b)
        {
            if (a < b)
            {
                return new IntSpan((int)Math.Floor(a), (int)Math.Ceiling(b));
            }
            return new IntSpan((int)Math.Floor(b), (int)Math.Ceiling(a));
        }
        public static readonly IntSpan Empty = new IntSpan(int.MaxValue, int.MinValue);
        public static readonly IntSpan Maximal = new IntSpan(int.MinValue, int.MaxValue);
        public bool IsEmpty()
        {
            return Max < Min;
        }
        public int Length()
        {
            return Math.Max(Max - Min + 1, 0);
        }
        public bool Contains(int x)
        {
            return x >= Min && x <= Max;
        }
        public IntSpan Intersect(IntSpan other)
        {
            return new IntSpan(Math.Max(Min, other.Min), Math.Min(Max, other.Max));
        }
    }
}
