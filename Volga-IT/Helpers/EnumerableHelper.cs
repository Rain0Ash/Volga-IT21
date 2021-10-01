// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;

namespace Volga_IT.Helpers
{
    public static class EnumerableHelper
    {
        public static IEnumerable<T> ChangeIf<T>(this IEnumerable<T> source, Func<T, T> selector, Boolean condition)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return condition ? source.Select(selector) : source;
        }
        
        public static IDictionary<T, Int64> LongCounter<T>(this IEnumerable<T> source) where T : notnull
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.GroupBy(item => item).ToDictionary(item => item.Key, item => item.LongCount());
        }
    }
}