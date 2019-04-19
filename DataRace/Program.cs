using System;

namespace DataRace
{
    // Invariant: A must be always equal to B
    public struct Foo
    {
        public long A { get; private set; }
        public long B { get; private set; }

        public Foo IncrementBy(long v)
        {
            A = A + v;
            B = B + v;
            return this;
        }

        public override string ToString()
        {
            return $"A = {A}, B = {B}";
        }
    }

    public static class Program
    {
        public static void Main()
        {
            var foo = new Foo();
            foo = Fooinator.SetFoo(foo, 1000,100);
            Console.WriteLine(foo);
        }
    }
}