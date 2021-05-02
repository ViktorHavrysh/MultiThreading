﻿using System;

namespace DataRace
{
    // Invariant: A must be always equal to B
    public struct Foo
    {
        // Private setters; encapsulation!
        public long A { get; private set; }
        public long B { get; private set; }

        // The only method that can modify this type
        public Foo IncrementBy(long v)
        {
            A += v;
            B += v;
            return this;
        }

        // Just for displaying Foo
        public override string ToString()
        {
            return $"A = {A}, B = {B}";
        }
    }

    public static class Program
    {
        public static void Main()
        {
            // Magic!
            Foo foo = Fooinator.SetFoo(5, 9);
            Console.WriteLine(foo);
        }
    }
}