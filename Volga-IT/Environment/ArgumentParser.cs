// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volga_IT.Environment.Interfaces;

namespace Volga_IT.Environment
{
    public class ArgumentParser : IArgumentParser
    {
        ReadOnlyCollection<String> IArgumentParser.Arguments
        {
            get
            {
                return new ReadOnlyCollection<String>(ArgumentsSet.ToArray());
            }
        }

        public IReadOnlySet<String> ArgumentsSet { get; }

        public ArgumentParser(IEnumerable<String> arguments)
        {
            if (arguments is null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            ArgumentsSet = arguments.Where(item => !String.IsNullOrEmpty(item)).Select(item => item.ToUpperInvariant()).ToHashSet();
        }
        
        public Boolean Contains(String argument)
        {
            if (argument is null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            return ArgumentsSet.Contains(argument);
        }
    }
}