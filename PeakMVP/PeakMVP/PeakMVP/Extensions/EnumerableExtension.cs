using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeakMVP.Extensions {
    public static class EnumerableExtension {

        public static async Task ForEachAsync<T>(this IEnumerable<T> sequence, Func<T, Task> func) {
            foreach (T item in sequence) {
                await func(item);
            }
        }
    }
}
