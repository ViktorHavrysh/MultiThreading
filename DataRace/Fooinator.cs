using System;
using System.Threading.Tasks;

namespace DataRace {
    public static class Fooinator
    {
        public static Foo SetFoo(Foo foo, long a, long b)
        {
            var distance = b - a;
            Foo result;
            while (true)
            {
                var copy = foo;
                if (copy.B - copy.A == distance)
                {
                    result = copy.IncrementBy(a - copy.A);
                    break;
                }

                var difference = Math.Abs(distance - Math.Abs(foo.A - foo.B));
                Task.Run(() => foo = foo.IncrementBy(difference));
            }

            return result;
        }
    }
}