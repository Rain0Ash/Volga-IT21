// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using Volga_IT.Models;

namespace Volga_IT.Extractor.Interfaces
{
    public interface IWordCounterRecordSorter
    {
        public IOrderedEnumerable<WordCounterRecord> Sort(IEnumerable<WordCounterRecord> source);
    }
}