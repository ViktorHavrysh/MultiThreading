using System;

namespace Synchronization
{
    // If no memory operations are reordered, Print can only print "Not initialized" or "42", but there are two possible
    // cases when Print could print a "0":
    // * Write 1 and Write 2 were reordered.
    // * Read 1 and Read 2 were reordered.
    public class Volatile
    {
        private int _data = 0;
        private volatile bool _initialized = false;

        public void Init()
        {
            _data = 42;          // Write 1
            _initialized = true; // Write 2
        }

        public void Print()
        {
            if (_initialized)             // Read 1
            {
                Console.WriteLine(_data); // Read 2
            }
            else
            {
                Console.WriteLine("Not initialized");
            }
        }
    }
    // If _initialized were not marked as volatile, both reorderings would be permitted. However, when _initialized is
    // marked as volatile, neither reordering is allowed! In the case of writes, you have an ordinary write followed by
    // a volatile write, and a volatile write can’t be reordered with a prior memory operation. In the case of the
    // reads, you have a volatile read followed by an ordinary read, and a volatile read can’t be reordered with a
    // subsequent memory operation.
    //
    // So, Print will never print "0", even if called concurrently with Init on a new instance of Volatile.
}