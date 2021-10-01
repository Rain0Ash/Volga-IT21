// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.ObjectModel;

namespace Volga_IT.Environment.Interfaces
{
    public interface IArgumentParser
    {
        public ReadOnlyCollection<String> Arguments { get; }

        public Boolean Contains(String argument);
    }
}