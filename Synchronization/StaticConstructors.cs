using System;

namespace Synchronization
{
    public class StaticConstructors
    {
        static StaticConstructors()
        {
            _obj = new object();
        }

        private static readonly int _value = 42;
        private static readonly object _obj;

        // If StaticConstructors.PrintValue is called from multiple threads concurrently, is it guaranteed that each
        // PrintValue call will print "42" and "false"? Or, could one of the calls also print "0" or "true"? Just as in
        // the previous case, you do get the behavior you'd expect: yes, each thread is guaranteed to print "42" and "false".
        public static void PrintValue()
        {
            Console.WriteLine(_value);
            Console.WriteLine(_obj == null);
        }
    }
}