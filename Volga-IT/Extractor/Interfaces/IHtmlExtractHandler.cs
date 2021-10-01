// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.IO;
using Volga_IT.Models;

namespace Volga_IT.Extractor.Interfaces
{
    public interface IHtmlExtractHandler
    {
        public Func<String, String>? CaseSelector { get; }
        
        public IEnumerable<WordCounterRecord> Extract(Stream stream, IHtmlTextExtractor extractor);

        public IEnumerable<WordCounterRecord>? Extract(Int64 hash, IHtmlTextExtractor extractor);

        public Int64 ComputeHash(Stream stream);
    }
}