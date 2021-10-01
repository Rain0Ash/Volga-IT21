// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Volga_IT.Extractor;
using Volga_IT.Extractor.Interfaces;
using Volga_IT.Helpers;

namespace Volga_IT_Tests.Extractor
{
    [TestFixture]
    public static class HtmlTextExtractorTests
    {
        private static void ExtractTestExample(String filename, String examplename)
        {
            using FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
            using FileStream example = new FileStream(examplename, FileMode.Open, FileAccess.Read, FileShare.None);
            
            IHtmlTextExtractor extractor = new HtmlTextExtractor(stream);
            using StreamReader reader = new StreamReader(example, Encoding.UTF8);
            
            IEnumerable<String> expected = extractor
                .Select(item => item.ToUpperInvariant())
                .LongCounter()
                .OrderByDescending(item => item.Value)
                .ThenBy(item => item.Key)
                .Select(item => $"{item.Key} - {item.Value}");

            foreach (String expect in expected)
            {
                String? line = reader.ReadLine();

                if (line is null)
                {
                    Assert.Fail();
                }

                Assert.AreEqual(expect, line);
            }
        }
        
        [Test]
        public static void ExtractTestExample1()
        {
            ExtractTestExample("file1.html", "extract1.txt");
        }
        
        [Test]
        public static void ExtractTestExample2()
        {
            ExtractTestExample("file2.html", "extract2.txt");
        }
        
        [Test]
        public static void ExtractTestExample3()
        {
            ExtractTestExample("file3.html", "extract3.txt");
        }
    }
}