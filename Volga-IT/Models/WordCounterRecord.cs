// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;

namespace Volga_IT.Models
{
    public record WordCounterRecord
    {
        public String Word { get; }
        public Int64 Count { get; }
        
        public WordCounterRecord(String word, Int64 count)
        {
            Word = word;
            Count = count;
        }

        public void Deconstruct(out String word, out Int64 count)
        {
            word = Word;
            count = Count;
        }
    }
}