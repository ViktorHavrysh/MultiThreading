using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
    // Interlocked operations are atomic operations that can be used to reduce locking in a multithreaded program.
    // Consider this simple thread-safe counter class:
    public class CounterLock
    {
        private int _value = 0;
        private readonly object _lock = new object();

        public int Increment()
        {
            lock (_lock)
            {
                _value++;
                return _value;
            }
        }
    }

    // Using Interlocked.Increment, you can rewrite the program like this:
    public class CounterInterlocked
    {
        private int _value = 0;

        public int Increment()
        {
            return Interlocked.Increment(ref _value);
        }
    }

    // As rewritten with Interlocked.Increment, the method should execute faster, at least on some architectures. In
    // addition to the increment operations, the Interlocked class exposes methods for various atomic operations: adding
    // a value, conditionally replacing a value, replacing a value and returning the original value, and so forth.

    public static class InterlockedExample
    {
        // 0 for false, 1 for true.
        private static int _usingResource;

        private const int NumThreadIterations = 5;
        private const int NumThreads = 10;
        private static readonly AsyncLocal<int> TaskNumber = new AsyncLocal<int>();

        public static void Try()
        {
            var tasks = new List<Task>();
            var rnd = new Random();

            for (var i = 0; i < NumThreads; i++)
            {
                var task = Task.Run(
                    () =>
                    {
                        TaskNumber.Value = i;
                        Processing();
                    });
                tasks.Add(task);
                // Wait a random amount of time before starting next thread.
                Thread.Sleep(rnd.Next(0, 1000));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static void Processing()
        {
            for (var i = 0; i < NumThreadIterations; i++)
            {
                UseResource();

                // Wait 1 second before next attempt.
                Thread.Sleep(1000);
            }
        }

        // A simple method that denies reentrancy.
        private static void UseResource()
        {
            // 0 indicates that the method is not in use.
            if (0 == Interlocked.Exchange(ref _usingResource, 1))
            {
                Console.WriteLine("{0} acquired the lock", TaskNumber.Value);

                // Code to access a resource that is not thread safe would go here.

                // Simulate some work
                Thread.Sleep(500);

                Console.WriteLine("{0} exiting lock", TaskNumber.Value);

                // Release the lock
                Interlocked.Exchange(ref _usingResource, 0);
            }
            else
            {
                Console.WriteLine("   {0} was denied the lock", TaskNumber.Value);
            }
        }
    }
}