using System;

namespace Synchronization
{
    public class StaticConstructors
    {
        private static readonly int _value = 42;
        private static readonly object _obj = new object();

        // If Test3.PrintValue is called from multiple threads concurrently, is it guaranteed that each PrintValue call
        // will print "42" and "false"? Or, could one of the calls also print "0" or "true"? Just as in the previous
        // case, you do get the behavior you’d expect: Each thread is guaranteed to print "42" and "false".
        public static void PrintValue()
        {
            Console.WriteLine(_value);
            Console.WriteLine(_obj == null);
        }
    }
}