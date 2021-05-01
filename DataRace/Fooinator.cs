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
        public static Foo SetFoo(long a, long b)
        {
            var foo = new Foo();
            var distance = b - a;
            Foo result;
            // The operation might not succeed the first time. This is why we try it as many times as needed.
            while (true)
            {
                // Create a value of Foo that is most likely to create a teared value that we want.
                // For example, if we want (2, 7), we create a (2, 2) and (7, 7) and try to write both into the same
                // variable from multiple threads
                var copy = foo;
                if (copy.B - copy.A == distance)
                {
                    result = copy.IncrementBy(a - copy.A);
                    break;
                }

                var difference = Math.Abs(distance - Math.Abs(foo.A - foo.B));
                // The impossible Foo value is created here. The assignment is a data race and causes tearing
                Task.Run(() => foo = foo.IncrementBy(difference));
            }

            return result;
        }
    }
}