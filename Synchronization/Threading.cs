using System;
using System.Threading.Tasks;

namespace Synchronization
{
    public class Threading
    {
        private static int _value;

        // When you examine the following code sample, you’d probably expect "42" to be printed to the screen. And, in
        // fact, your intuition would be correct. This code sample is guaranteed to print "42."
        //
        // It might be surprising that this case even needs to be mentioned, but in fact there are possible
        // implementations of Task.Run() that would allow "0" to be printed instead of "42," at least in theory. After
        // all, there are two threads communicating via a non-volatile field, so memory operations can be reordered.
        public static void Run()
        {
            _value = 42;
            var t = Task.Run(() => Console.WriteLine(_value));
            t.Wait();
        }

        // The Task.Run() implementation must ensure that the write to _value on Thread 1 will not move after
        // <start task t>, and the read from s_value on Thread 2 will not move before <task t starting>. And, in fact,
        // the Task.Run() and Task.Factory.StartNew() APIs really do guarantee this.
        //
        // All other threading APIs in the .NET Framework, such as Thread.Start() and ThreadPool.QueueUserWorkItem(),
        // also make a similar guarantee. In fact, nearly every threading API must have some barrier semantics in order
        // to function correctly. These are almost never documented, but can usually be deduced simply by thinking about
        // what the guarantees would have to be in order for the API to be useful.
    }
}