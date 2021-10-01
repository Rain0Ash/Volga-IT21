// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Text;
using NUnit.Framework;
using Volga_IT.Helpers;

namespace Volga_IT_Tests.Helpers
{
    [TestFixture]
    public static class StringBuilderHelperTests
    {
        [Test]
        public static void LastIndexOfTest()
        {
            StringBuilder builder = new StringBuilder("Volga-o-IT");

            Assert.AreEqual(-1, builder.LastIndexOf('\0'));
            Assert.AreEqual(-1, builder.LastIndexOf('0'));
            Assert.AreEqual(-1, builder.LastIndexOf('P'));
            
            Assert.AreEqual(0, builder.LastIndexOf('V'));
            Assert.AreEqual(2, builder.LastIndexOf('l'));
            Assert.AreEqual(6, builder.LastIndexOf('o'));
            Assert.AreEqual(7, builder.LastIndexOf('-'));
            Assert.AreEqual(9, builder.LastIndexOf('T'));
        }
    }
}