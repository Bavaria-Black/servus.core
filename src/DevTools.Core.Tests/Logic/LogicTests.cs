using DevTools.Core.Logic;
using DevTools.Core.Logic.Blocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Tests.Logic
{
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void BasicTest()
        {
            var add5Block = new AddConstantBlock(5);
            var add10Block = new AddConstantBlock(10);
            var debugBlock = new DebugBlock();

            var compare = new CompareBlock<IntMessage>((m) =>
            {
                return m.Value == 5;
            });

            _ = new LogicConnection<IntMessage>(add5Block, 0, debugBlock);
            _ = new LogicConnection<IntMessage>(add5Block, 0, compare);

            // output 1 is only triggered when false
            _ = new LogicConnection<IntMessage>(compare, 1, debugBlock);

            var msg = new IntMessage(0);
            add5Block.TriggerInput(msg);
            Assert.AreEqual(5, msg.Value);

            // output 0 is only triggered when true
            _ = new LogicConnection<IntMessage>(compare, 0, add10Block);

            // should result in 15 because comparer returned true
            msg = new IntMessage(0);
            add5Block.TriggerInput(msg);
            Assert.AreEqual(15, msg.Value);

            // should result in 6 because comparer returned false
            msg = new IntMessage(1);
            add5Block.TriggerInput(msg);
            Assert.AreEqual(6, msg.Value);
        }
    }
}
