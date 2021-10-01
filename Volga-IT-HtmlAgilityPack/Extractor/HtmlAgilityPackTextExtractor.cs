// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Volga_IT.Extractor.Interfaces;
using Volga_IT.Helpers;

namespace Volga_IT.Extractor
{
    /// <summary>
    /// Example extractor using HtmlAgilityPack
    /// </summary>
    public class HtmlAgilityPackTextExtractor : IHtmlTextExtractor
    {
        protected Stream Stream { get; }
        protected IReadOnlySet<Char> Separators { get; }

        public Encoding? Encoding { get; init; }

        private readonly Int32 _size = 4096;
        protected Int32 BufferSize
        {
            get
            {
                return _size;
            }
            init
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Argument can't be less or equals zero");
                }
                
                _size = value;
            }
        }

        public HtmlAgilityPackTextExtractor(Stream stream)
            : this(stream, null)
        {
        }

        public HtmlAgilityPackTextExtractor(Stream stream, IReadOnlySet<Char>? separators)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream must be readable", nameof(stream));
            }

            Stream = stream;
            Separators = separators ?? HtmlHelper.DefaultSeparators;
        }

        protected virtual IEnumerable<String> Extract()
        {
            using TextReader reader = new StreamReader(Stream, Encoding ?? Encoding.UTF8, false, BufferSize, true);
            HtmlDocument document = new HtmlDocument();
            document.Load(reader);

            Char[] separators = Separators.ToArray();

            return document.DocumentNode.DescendantsAndSelf()
                .Where(item => item.NodeType == HtmlNodeType.Text)
                .Select(item => HtmlEntity.DeEntitize(item.InnerText))
                .Select(item => item.Trim())
                .SelectMany(item => item.Split(separators, StringSplitOptions.RemoveEmptyEntries))
                .Where(item => item.Length > 0);
        }

        public IEnumerator<String> GetEnumerator()
        {
            return Extract().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}