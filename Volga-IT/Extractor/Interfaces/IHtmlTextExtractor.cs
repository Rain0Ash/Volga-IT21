// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Text;

namespace Volga_IT.Extractor.Interfaces
{
    public interface IHtmlTextExtractor : IEnumerable<String>
    {
        public Encoding? Encoding { get; }
    }
}