using DevTools.Core.Parsing.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DevTools.Core.Tests.Parsing.Text
{
    [TestClass]
    public class LineKeyValueParserTests
    {
        private LineKeyValueParser _parser;

        [TestInitialize]
        public void Init()
        {
            _parser = new LineKeyValueParser('=');
        }

        [TestMethod]
        [DataRow("h=z3FaW23KmdHWIyBA99Cztqu7MHc=", "h", "z3FaW23KmdHWIyBA99Cztqu7MHc=")]
        [DataRow("sl=25", "sl", "25")]
        public void ParseLineTest(string input, string key, string value)
        {
            var parsed = _parser.ParseLine(input);
            Assert.AreEqual(key, parsed.Key);
            Assert.AreEqual(value, parsed.Value);
        }

        [TestMethod]
        public void ParseMultiline()
        {
            var lines = "h=z3FaW23KmdHWIyBA99Cztqu7MHc=" + Environment.NewLine +
                        "t=2019-09-15T09:13:15Z0817" + Environment.NewLine +
                        "otp=cccccclibubjhkuttefctkgejjgerdjfihbkhtddivju" + Environment.NewLine +
                        "nonce=aef3a7835277a28da831005c2ae3b919e2076a62" + Environment.NewLine +
                        "sl=25" + Environment.NewLine +
                        "status=OK" + Environment.NewLine;

            var results = _parser.Parse(lines);
            var r = results.GetEnumerator();
            r.MoveNext();
            Assert.AreEqual("h", r.Current.Key);
            Assert.AreEqual("z3FaW23KmdHWIyBA99Cztqu7MHc=", r.Current.Value);

            r.MoveNext();
            Assert.AreEqual("t", r.Current.Key);
            Assert.AreEqual("2019-09-15T09:13:15Z0817", r.Current.Value);

            r.MoveNext();
            Assert.AreEqual("otp", r.Current.Key);
            Assert.AreEqual("cccccclibubjhkuttefctkgejjgerdjfihbkhtddivju", r.Current.Value);

            r.MoveNext();
            Assert.AreEqual("nonce", r.Current.Key);
            Assert.AreEqual("aef3a7835277a28da831005c2ae3b919e2076a62", r.Current.Value);

            r.MoveNext();
            Assert.AreEqual("sl", r.Current.Key);
            Assert.AreEqual("25", r.Current.Value);

            r.MoveNext();
            Assert.AreEqual("status", r.Current.Key);
            Assert.AreEqual("OK", r.Current.Value);
        }
    }
}
