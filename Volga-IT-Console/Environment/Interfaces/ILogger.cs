// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;

namespace Volga_IT.Environment.Interfaces
{
    public interface ILogger
    {
        public LoggerMessageLevel Level { get; set; }
        
        public Boolean Log(String message, LoggerMessageLevel level);
    }
}