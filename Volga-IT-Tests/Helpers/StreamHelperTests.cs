// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Volga_IT.Helpers;

namespace Volga_IT_Tests.Helpers
{
    [TestFixture]
    public static class StreamHelperTests
    {
        public static void TryReadCharTest()
        {
            const String example = "Специальные тесты для VOLGA-IT'21";
            
            using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(example));

            Int32 i = 0;
            while (stream.TryReadChar() is Char character)
            {
                Assert.AreEqual(example[i++], character);
            }
        }
    }
}