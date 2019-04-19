using System;
using System.Threading.Tasks;

namespace DataRace
{
    // Invariant: A must be always equal to B
    public struct Foo
    {
        public long A { get; private set; }
        public long B { get; private set; }

        public Foo Increment()
        {
            return new Foo {A = A + 1, B = B + 1};
        }

        public override string ToString()
        {
            var invariant = A == B ? "upheld" : $"broken by {Math.Abs(A - B)}";
            return $"A = {A}, B = {B}, Invariant: {invariant}";
        }
    }


    public static class Program
    {
        public static void Main()
        {
            var foo = new Foo();
            Parallel.For(0, 1_000_000, i => { foo = foo.Increment(); });
            Console.WriteLine(foo);
        }
    }
}