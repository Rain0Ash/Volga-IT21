using System;
using System.Collections.Generic;
using System.IO;
using Volga_IT.Database;
using Volga_IT.Environment;
using Volga_IT.Environment.Interfaces;
using Volga_IT.Extractor;
using Volga_IT.Extractor.Interfaces;
using Volga_IT.Helpers;
using Volga_IT.Models;

namespace Volga_IT
{
    public static class Program
    {
        public static void Main(String[] args)
        {
            try
            {
                Initialize(args);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Filepath not found");
            }
            catch (Exception exception)
            {
                Logger.Default.Log(exception.ToString(), LoggerMessageLevel.Fatal);
                System.Environment.ExitCode = 1;
            }
        }

        //Производим инициализацию
        public static void Initialize(String[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            IArgumentParser parser = new ArgumentParser(args);

            ArgumentHandler handler = new ArgumentHandler(parser);

            if (handler.HandleHelpRequestArgument())
            {
                Console.WriteLine(ArgumentHandler.HelpRequestArgumentText);
                return;
            }
            
            if (args.Length <= 0 || !PathHelper.IsExistAsFile(args[0]))
            {
                throw new FileNotFoundException();
            }

            if (!InitializeLogger(handler))
            {
                Console.WriteLine("Can't create file logger.");
            }

            Execute(args[0], handler);
        }

        public static Boolean InitializeLogger(ArgumentHandler handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            FileInfo? info = handler.GetLogFileInfo();

            Boolean trace = handler.HandleTraceLoggingLevelArgument();

            if (info is null)
            {
                if (trace)
                {
                    Logger.Default.Level = LoggerMessageLevel.Trace;
                }

                return true;
            }

            try
            {
                StreamWriter writer = info.CreateText();
                Logger.SetDefaultLogger(new Logger(writer) { Level = trace ? LoggerMessageLevel.Trace : LoggerMessageLevel.Warning });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static HtmlContext? InitializeDatabaseContext(ArgumentHandler handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return handler.GetDatabaseModel<ApplicationHtmlContext>();
        }

        public static void Execute(String filepath, ArgumentHandler handler)
        {
            if (filepath is null)
            {
                throw new ArgumentNullException(nameof(filepath));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Logger.Default.Log("Opening filestream", LoggerMessageLevel.Trace);

            using FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.None);

            Logger.Default.Log("Filestream opened", LoggerMessageLevel.Trace);

            IHtmlTextExtractor extractor = new HtmlTextExtractor(stream);
            
            Logger.Default.Log("Extractor created", LoggerMessageLevel.Trace);

            IEnumerable<WordCounterRecord> words;

            Func<String, String> caseselector = handler.GetCaseArgumentHandler();
            
            Logger.Default.Log("Initializing database context", LoggerMessageLevel.Trace);
            
            using HtmlContext? context = InitializeDatabaseContext(handler);

            if (context is null)
            {
                Logger.Default.Log("Database context is not initialized", handler.Contains(ArgumentHandler.UseDatabaseArgument) ? LoggerMessageLevel.Error : LoggerMessageLevel.Trace);
                
                HtmlExtractHandler extract = new HtmlExtractHandler { CaseSelector = caseselector };
                words = extract.Extract(stream, extractor);
            }
            else
            {
                Logger.Default.Log("Database context is successful initialized", LoggerMessageLevel.Trace);
                
                DatabaseHtmlExtractHandler extract = new DatabaseHtmlExtractHandler(context) { CaseSelector = caseselector };
                words = extract.ExtractAndSaveToDatabase(stream, extractor);
            }

            Logger.Default.Log("Extract words from html file", LoggerMessageLevel.Trace);

            foreach ((String word, Int64 count) in new WordCounterRecordSorter().Sort(words))
            {
                Console.WriteLine($"{word} - {count}");
            }

            Logger.Default.Log("Words from html file successful extracted", LoggerMessageLevel.Trace);

            Logger.Default.Log("Program exit", LoggerMessageLevel.Trace);
        }
    }
}