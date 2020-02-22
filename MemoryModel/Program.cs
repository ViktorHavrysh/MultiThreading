using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MemoryModel
{
    public static class Program
    {
        private static volatile bool _loop = true;

        public static void Main()
        {
            var sw = Stopwatch.StartNew();

            // Set Loop to false on another thread
            Task.Run(
                () =>
                {
                    Thread.Sleep(5000);
                    _loop = false; // Write A
                });

            long counter = 0;

            // Monitor the state of the loop
            Task.Run(
                () =>
                {
                    while (true)
                    {
                        Console.WriteLine($"State: Continue = {_loop}, Loops: {counter}, Elapsed: {sw.Elapsed.TotalMilliseconds} ms");
                        Thread.Sleep(500);
                    }
                });

            // Poll the _loop field until it is set to false
            while (_loop) // Read A
            {
                // Do some work
                counter++;
            }

            Console.WriteLine("We finished! {0} loops", counter);
        }
    }

    // Polling loop is a pattern that's generally not recommended but--somewhat unfortunately--frequently used in
    // practice

    // In the previous example, the main thread loops, polling a particular non-volatile field. A helper thread sets the
    // field in the meantime, but the main thread may never see the updated value.

    // One source of complexity in multithreaded programming is that the compiler and the hardware can subtly transform
    // a program's memory operations in ways that don't affect the single-threaded behavior, but might affect the
    // multithreaded behavior. Consider the following:
    public class DataInit
    {
        private bool _initialized;
        private int _data;

        public void Init()
        {
            _data = 42; // Write A
            _initialized = true; // Write B
        }

        public string Print()
        {
            if (_initialized) // Read A
            {
                return _data.ToString(); // Read B
            }
            else
            {
                return "Not initialized";
            }
        }

        // Suppose Init and Print are called in parallel (that is, on different threads) on a new instance of DataInit.
        // If you examine the code of Init and Print, it may seem that Print can only output "42" or "Not initialized".
        // However, Print can also output "0".

        // The C# memory model permits reordering of memory operations in a method, as long as the behavior of
        // single-threaded execution doesn't change. For example, the compiler and the processor are free to reorder
        // the Init method operations.

        public void InitReordering()
        {
            _initialized = true; // Write B
            _data = 42; // Write A
        }

        // This reordering wouldn't change the behavior of the Init method in a single-threaded program. In a
        // multithreaded program, however, another thread might read _initialized and _data fields after Init has
        // modified one field but not the other, and then the reordering could change the behavior of the program. As a
        // result, the Print method could end up outputting a 0.

        // The reordering of Init isn't the only possible source of trouble in this code sample. Even if the Init writes
        // don't end up reordered, the reads in the Print method could be transformed:

        public string PrintReordering()
        {
            var d = _data; // Read B
            if (_initialized) // Read A
            {
                return d.ToString();
            }
            else
            {
                return "Not initialized";
            }
        }

        // Just as with the reordering of writes, this transformation has no effect in a single-threaded program, but
        // might change the behavior of a multithreaded program. And, just like the reordering of writes, the reordering
        // of reads can also result in a 0 printed to the output.
    }

    // The C# programming language provides volatile fields that constrain how memory operations can be reordered.
    // The ECMA specification states that volatile fields provide acquire­release semantics.

    // A read of a volatile field has acquire semantics, which means it can't be reordered with subsequent operations.
    // The volatile read forms a one-way fence: preceding operations can pass it, but subsequent operations can't.
    // Consider this example:
    public class AcquireSemanticsExample
    {
        private int _a;
        private volatile int _b;
        private int _c;

        public void Foo()
        {
            var a = _a; // Read 1
            var b = _b; // Read 2 (volatile)
            var c = _c; // Read 3
        }

        // Read 1 and Read 3 are non-volatile, while Read 2 is volatile. Read 2 can't be reordered with Read 3, but it
        // can be reordered with Read 1. These are the valid reorderings of the Foo body:

        public void FooReordering1()
        {
            var b = _b; // Read 2 (volatile)
            var a = _a; // Read 1
            var c = _c; // Read 3
        }

        public void FooReordering2()
        {
            var b = _b; // Read 2 (volatile)
            var c = _c; // Read 3
            var a = _a; // Read 1
        }
    }

    // A write of a volatile field, on the other hand, has release semantics, and so it can't be reordered with prior
    // operations. A volatile write forms a one-way fence, as this example demonstrates:

    public class ReleaseSemanticsExample
    {
        private int _a;
        private volatile int _b;
        private int _c;

        private void Foo()
        {
            _a = 1; // Write 1
            _b = 1; // Write 2 (volatile)
            _c = 1; // Write 3
        }

        // Write 1 and Write 3 are non-volatile, while Write 2 is volatile. Write 2 can't be reordered with Write 1, but
        // it can be reordered with Write 3
        public void FooReordering1()
        {
            _a = 1; // Write 1
            _c = 1; // Write 3
            _b = 1; // Write 2 (volatile)
        }

        public void FooReordering2()
        {
            _c = 1; // Write 3
            _a = 1; // Write 1
            _b = 1; // Write 2 (volatile)
        }
    }

    // Some compiler optimizations may introduce or eliminate certain memory operations. For example, the compiler might
    // replace repeated reads of a field with a single read. Similarly, if code reads a field and stores the value in a
    // local variable and then repeatedly reads the variable, the compiler could choose to repeatedly read the field
    // instead.
}
