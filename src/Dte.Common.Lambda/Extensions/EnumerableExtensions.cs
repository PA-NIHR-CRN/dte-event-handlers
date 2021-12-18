using System;
using System.Collections.Generic;
using System.Linq;

namespace Dte.Common.Lambda.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> GetByBatch<T>(this IEnumerable<T> source, int batchSize)
        {
            if (source == null)
            {
                yield break;
            }

            using var enumerator = source.GetEnumerator();
            
            while (true)
            {
                var batch = new List<T>(batchSize);
                var count = 0;

                while (count < batchSize && enumerator.MoveNext())
                {
                    batch.Add(enumerator.Current);
                    count++;
                }

                if (count > 0)
                {
                    yield return batch;
                }
                else
                {
                    yield break;
                }
            }
        }

        public static void GetNext<T>(ref this List<T>.Enumerator source)
        {
            if (!source.MoveNext())
            {
                throw new Exception("Can not move next on the Enumerator");
            }
        }

        public static T[] GetNext<T>(ref this List<T>.Enumerator source, int numberOfItems)
        {
            var bytes = new List<T>();
            foreach (var _ in Enumerable.Range(1, numberOfItems))
            {
                source.GetNext();

                bytes.Add(source.Current);
            }

            return bytes.ToArray();
        }
    }
}