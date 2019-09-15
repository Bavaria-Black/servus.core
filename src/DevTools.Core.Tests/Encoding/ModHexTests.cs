using DevTools.Core.Encoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Tests.Encoding
{
    [TestClass]
    public class ModHexTests
    {
        [TestMethod]
        public void Encode()
        {
            var result = ModHex.EncodeFromAscii("test");
            Assert.AreEqual("ifhgieif", result);
        }
        [TestMethod]
        public void Decode()
        {
            var result = ModHex.DecodeToAscii("ifhgieif");
            Assert.AreEqual("test", result);
        }
    }
}
