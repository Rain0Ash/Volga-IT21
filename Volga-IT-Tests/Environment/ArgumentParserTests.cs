// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Volga_IT.Environment;

namespace Volga_IT_Tests.Environment
{
    [TestFixture]
    public static class ArgumentParserTests
    {
        public static IEnumerable<String> Arguments { get; } = new List<String> { "-database", "-Value", "-ANYTHING", "-VoLgA-IT" };

        [Test]
        public static void ParserContainsTest()
        {
            ArgumentParser parser = new ArgumentParser(Arguments);
            Assert.IsTrue(parser.Contains("-DATABASE"));
            Assert.IsTrue(parser.Contains("-VALUE"));
            Assert.IsTrue(parser.Contains("-ANYTHING"));
            Assert.IsTrue(parser.Contains("-VOLGA-IT"));
        }
        
        [Test]
        public static void ParserNotContainsTest()
        {
            ArgumentParser parser = new ArgumentParser(Arguments);
            Assert.IsFalse(parser.Contains("-NONE"));
            Assert.IsFalse(parser.Contains("TESTVALUE"));
            Assert.IsFalse(parser.Contains("-TEST"));
            Assert.IsFalse(parser.Contains(String.Empty));
        }
    }
}