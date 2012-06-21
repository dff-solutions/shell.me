using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            if (action == null)
                throw new ArgumentNullException("action");

            foreach (var item in enumerable)
            {
                action(item);
            }
        }
    }
}
