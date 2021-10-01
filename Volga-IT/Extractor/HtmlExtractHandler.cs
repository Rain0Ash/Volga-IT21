// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NetExtender.Crypto.Hashes.XXHash;
using Volga_IT.Extractor.Interfaces;
using Volga_IT.Helpers;
using Volga_IT.Models;

namespace Volga_IT.Database
{
    public class HtmlExtractHandler : IHtmlExtractHandler
    {
        public Func<String, String>? CaseSelector { get; init; }

        public virtual IEnumerable<WordCounterRecord> Extract(Stream stream, IHtmlTextExtractor extractor)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (extractor is null)
            {
                throw new ArgumentNullException(nameof(extractor));
            }

            return ExtractFromHtml(extractor);
        }
        
        public virtual IEnumerable<WordCounterRecord> Extract(Int64 hash, IHtmlTextExtractor extractor)
        {
            return ExtractFromHtml(extractor);
        }
        
        protected virtual IEnumerable<WordCounterRecord> ExtractFromHtml(IHtmlTextExtractor extractor)
        {
            if (extractor is null)
            {
                throw new ArgumentNullException(nameof(extractor));
            }

            // Тут можно использовать какую-либо лингвистическую библиотеку для приведения окончаний слов к единому знаменателю.
            return extractor
                .Select(CaseSelector ?? (item => item.ToUpperInvariant()))
                .LongCounter()
                .Select(pair => new WordCounterRecord(pair.Key, pair.Value));
        }
        
        // При желании можно заменить хеш на любой другой. Переписывать относительно немного (заменить алгоритм и подправить типизацию).
        // XXHash был выбран т.к. это самый быстрый алгоритм некриптостойкого хеширования размером 64 бит. Я думаю 64 бит вполне достаточно для данного задания.
        public virtual Int64 ComputeHash(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream must be readable", nameof(stream));
            }

            if (!stream.CanSeek)
            {
                throw new ArgumentException("Stream must be seekable", nameof(stream));
            }
            
            stream.Seek(0, SeekOrigin.Begin);
            UInt64 hash = XXHash64.ComputeHash(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return unchecked((Int64) hash);
        }
    }
}