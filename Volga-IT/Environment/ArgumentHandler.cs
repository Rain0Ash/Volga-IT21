// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Globalization;
using System.IO;
using Volga_IT.Environment.Interfaces;
using Volga_IT.Helpers;

namespace Volga_IT.Environment
{
    public class ArgumentHandler
    {
        public const String LowerCaseArgument = "-LOWER";
        public const String NormalCaseArgument = "-NORMAL";
        public const String CurrentCultureArgument = "-CULTURE";
        public const String LogToFileArgument = "-LOGTOFILE";
        public const String LoggingTraceLevelArgument = "-TRACE";
        public const String UseDatabaseArgument = "-DATABASE";
        public const String HelpRequestArgument = "-HELP";
        
        // Можно перенести в ресурсы программы
        public const String HelpRequestArgumentText = "-lower - lowercasing words on html parsing" + "\n" +
                                                      "-normal - save words case on html parsing" + "\n" +
                                                      "-upper - uppercase words on html parsing [DEFAULT MODE]" + "\n" +
                                                      "-culture - use current culture instead of invariant" + "\n" +
                                                      "-logtofile - enable logging exception to file instead of console" + "\n" +
                                                      "-trace - enable logging trace level" + "\n" +
                                                      "-database - enable save statistics to sqlite database" + "\n" +
                                                      "-help - show help";
        
        private IArgumentParser Parser { get; }

        public ArgumentHandler(IArgumentParser parser)
        {
            Parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public Boolean Contains(String argument)
        {
            return Parser.Contains(argument);
        }

        public Func<String, String> GetCaseArgumentHandler()
        {
            return (Contains(LowerCaseArgument), Contains(NormalCaseArgument), Contains(CurrentCultureArgument)) switch
            {
                (false, false, false) => item => item.ToUpperInvariant(),
                (false, false, true) => item => item.ToUpper(CultureInfo.CurrentCulture),
                (false, true, _) => item => item,
                (true, false, false) => item => item.ToLowerInvariant(),
                (true, false, true) => item => item.ToLower(CultureInfo.CurrentCulture),
                (true, true, _) => item => item
            };
        }

        public Boolean HandleHelpRequestArgument()
        {
            return Contains(HelpRequestArgument);
        }

        public Boolean HandleTraceLoggingLevelArgument()
        {
            return Contains(LoggingTraceLevelArgument);
        }
        
        // Можно прочитать место куда будет записываться файл.
        // Но для работы с аргументами командной строки следует использовать специальную библиотеку, а не писать велосипед.
        public FileInfo? GetLogFileInfo()
        {
            if (!Contains(LogToFileArgument))
            {
                return null;
            }
            
            String? directory = ApplicationHelper.Directory;

            return directory is not null ? new FileInfo(Path.Join(directory, "logger.log")) : null;
        }
    }
}