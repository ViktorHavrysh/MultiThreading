using System;
using System.Threading.Tasks;

namespace DataRace
{
    // Invariant: A must be equal to B
    internal struct Test
    {
        private long _a;
        private long _b;

        public long A
        {
            get => _a;
            set
            {
                _a = value;
                _b = value;
            }
        }

        public long B
        {
            get => _b;
            set
            {
                _a = value;
                _b = value;
            }
        }
    }

    public static class Program
    {
        private static Test _testStatic;
        private static readonly Random _random = new Random();

        public static void Main(string[] _)
        {
            var test1 = new Test {A = 1};
            var test2 = new Test {A = 2};

            for (int i = 0; i < 1_000_000; i++)
            {
                Task.Run(() =>
                {
                    _testStatic = i % 2 == 0 ? test1 : test2;
                    var copy = _testStatic;
                    if (copy.A != copy.B)
                    {
                        Console.WriteLine("Invariant broken!");
                    }
                });
            }
        }
    }
}