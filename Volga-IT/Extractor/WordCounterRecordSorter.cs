// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using Volga_IT.Extractor.Interfaces;
using Volga_IT.Models;

namespace Volga_IT.Extractor
{
    public class WordCounterRecordSorter : IWordCounterRecordSorter
    {
        /// <summary>
        /// Sort <see cref="WordCounterRecord"/> of <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The source of <see cref="WordCounterRecord"/></param>
        /// <returns>Sorted <see cref="IEnumerable{T}"/> of <see cref="WordCounterRecord"/></returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is null</exception>
        public virtual IOrderedEnumerable<WordCounterRecord> Sort(IEnumerable<WordCounterRecord> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.OrderByDescending(item => item.Count).ThenBy(item => item.Word);
        }
    }
}