// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Globalization;
using System.IO;
using Volga_IT.Environment.Interfaces;

namespace Volga_IT.Environment
{
    public class Logger : ILogger
    {
        public static ILogger Default { get; private set; } = new Logger(Console.Out);

        public static ILogger SetDefaultLogger(ILogger logger)
        {
            Default = logger ?? throw new ArgumentNullException(nameof(logger));
            return Default;
        }
        
        protected TextWriter Writer { get; }

        public LoggerMessageLevel Level { get; set; } = LoggerMessageLevel.Warning;

        public Logger(TextWriter writer)
        {
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public virtual Boolean Log(String message, LoggerMessageLevel level)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (level < Level)
            {
                return false;
            }

            try
            {
                Writer.WriteLine($"[{level.ToString()}] {DateTime.Now.ToString(CultureInfo.InvariantCulture)}: {message}");
                Writer.Flush();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}