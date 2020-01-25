using System.Linq;
using DevTools.Core.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevTools.Core.Tests.Parsing
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        [DataRow(null, 0)]
        [DataRow("", 0)]
        [DataRow("         ", 1)]
        [DataRow(" ", 1)]
        [DataRow(" \r\n", 1)]
        [DataRow(" \r\n ", 2)]
        [DataRow("test\rtest", 2)]
        [DataRow(" This\n is\r a\r\n Test!", 4)]
        [DataRow(" 1\n\r3", 3)]
        public void GetLinesReturnsExpectedNumberOfLines(string text, int lines)
        {
            var numberOfLines = text.GetLines().Count();
            Assert.AreEqual(lines, numberOfLines);
        }
    }
}