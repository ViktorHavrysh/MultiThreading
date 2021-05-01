using System;

namespace Synchronization
{
    // Locking is typically the easiest way to share data between threads. If you use locks correctly, you basically
    // don't have to worry about any of the memory model messiness. Whenever a thread acquires a lock, the CLR ensures
    // that the thread will see all updates made by the thread that held the lock earlier.
    public class Locking
    {
        // These two fields need to be guarded by the _lock object
        private int _a;
        private int _b;
        private readonly object _lock = new object();

        public void Set()
        {
            lock (_lock)
            {
                _a = 1;
                _b = 1;
            }
        }

        public void Print()
        {
            lock (_lock)
            {
                var b = _b;
                var a = _a;
                Console.WriteLine("{0} {1}", a, b);
            }
        }
    }
    // Adding a lock that Print and Set acquire provides a simple solution. Now, Set and Print execute exclusively with
    // respect to each other. The lock statement guarantees that the bodies of Print and Set will appear to execute in
    // a sequential order, even if they’re called from multiple threads.
}