using DevTools.Core.Flows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Tests.Flows
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void GetNotExistingValueTest()
        {
            var msg = new Message();
            Assert.AreEqual(0, msg.GetValue<int>("notExists"));
            Assert.IsFalse(msg.GetValue<bool>("notExistsEther"));
            Assert.IsNull(msg.GetValue<object>("nope"));
        }

        [TestMethod]
        public void DuplicateTest()
        {
            var msg = new Message();
            msg.SetValue<int>("test", 555);

            var dupe = msg.Duplicate();
            Assert.AreEqual(555, dupe.GetValue<int>("test"));
        }
    }
}
