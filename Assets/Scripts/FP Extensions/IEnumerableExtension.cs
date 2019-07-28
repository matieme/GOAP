using System;
using System.Collections.Generic;

namespace FP
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<R> StepAggregate<T, R>(this IEnumerable<T> list, R seed, Func<R, T, R> aggregator)
        {
            var accum = seed;

            foreach (var element in list)
            {
                accum = aggregator(accum, element);
                yield return accum;
            }
        }

        public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            foreach (var element in list)
            {
                yield return element;

                if (predicate(element))
                    yield break;
            }
        }
    }
}
