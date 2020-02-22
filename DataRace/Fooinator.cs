using System;
using System.Threading.Tasks;

namespace DataRace
{
    public static class Fooinator
    {
        // The C# ECMA specification guarantees that the following types will be written atomically:
        // reference types, bool, char, byte, sbyte, short, ushort, uint, int and float.
        // Values of other types -- including user-defined value types -- could be written into memory in multiple
        // atomic writes. As a result, a reading thread could observe a torn value consisting of pieces of different
        // values.
        public static Foo SetFoo(Foo foo, long a, long b)
        {
            var distance = b - a;
            Foo result;
            while (true)
            {
                var copy = foo;
                if (copy.B - copy.A == distance)
                {
                    result = copy.IncrementBy(a - copy.A);
                    break;
                }

                var difference = Math.Abs(distance - Math.Abs(foo.A - foo.B));
                Task.Run(() => foo = foo.IncrementBy(difference));
            }

            return result;
        }
    }
}