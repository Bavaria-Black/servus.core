using DevTools.Core.Encoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevTools.Core.Tests.Encoding
{
    [TestClass]
    public class ModHexTests
    {
        [TestMethod]
        public void Encode()
        {
            var endcoding = new ModHexEncoding();
            var bytes = endcoding.GetBytes("test");
            var result = System.Text.Encoding.ASCII.GetString(bytes);

            Assert.AreEqual("ifhgieif", result);
        }

        [TestMethod]
        public void Decode()
        {
            var endcoding = new ModHexEncoding();
            var bytes = System.Text.Encoding.ASCII.GetBytes("ifhgieif");
            var result = endcoding.GetString(bytes);

            Assert.AreEqual("test", result);
        }
    }
}
