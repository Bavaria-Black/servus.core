using DevTools.Core.Flows;
using DevTools.Core.Flows.Blocks;
using DevTools.Core.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace DevTools.Core.Tests.Flows
{
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void BasicBlockAndConnectionTest()
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

        [TestMethod]
        public void FlowTest()
        {
            bool customBlockReached = false;
            var msg = new IntMessage(5);
            var flow = new Flow<IntMessage>();
            var add10Block = new AddConstantBlock(10);
            var compareBlock = new CompareBlock<IntMessage>(m => m.Value == 3);
            var debugBlock = new DebugBlock();
            var customBlock = new FunctionBlock<IntMessage>(1, (m) =>
            {
                customBlockReached = true;
                return new[] { m };
            });

            flow.AddBlock(add10Block);
            flow.AddBlock(compareBlock);
            flow.AddBlock(customBlock);
            flow.AddBlock(debugBlock);

            flow.AddConnection(add10Block, 0, compareBlock);
            flow.AddConnection(compareBlock, 0, customBlock);
            flow.AddConnection(compareBlock, 1, debugBlock);

            flow.AddEntryBlock(add10Block);

            flow.Trigger(msg);
            Assert.IsFalse(customBlockReached);

            msg.Value = -7;
            flow.Trigger(msg);
            Assert.IsTrue(customBlockReached);
        }

        [TestMethod]
        public void Serialize()
        {
            var msg = new IntMessage(5);
            var flow = new Flow<IntMessage>();
            flow.Context.SetValue("test", 100);

            var add10Block = new AddConstantBlock(10);
            var debugBlock = new DebugBlock();

            flow.AddBlock(add10Block);
            flow.AddBlock(debugBlock);

            flow.AddConnection(add10Block, 0, debugBlock);
            flow.AddEntryBlock(add10Block);

            var serializer = new Serialization.JsonSerializer();
            var json = FlowFactory.Serialize(serializer, flow);
            Assert.IsFalse(string.IsNullOrEmpty(json));

            var f1 = FlowFactory.Deserialize<IntMessage>(serializer, json);
            Assert.AreEqual(100, f1.Context.GetValue<long>("test"));

            f1.Trigger(msg);
            Assert.AreEqual(15, msg.Value);
        }

        [TestMethod]
        public void ScriptFlow()
        {
            var flow = new Flow<IntMessage>();
            flow.Context.RegisterScriptRuntimeProvider(new PowerShellRuntimeProvider());
            var scriptBlock = new ScriptBlock<IntMessage>(1, 
                "$msg.SetValue(\"test\", 5); \r\n" +
                "$msg.Value = 100; \r\n" +
                "$output = @($msg); \r\n");

            flow.AddBlock(scriptBlock);
            flow.AddEntryBlock(scriptBlock);

            var msg = new IntMessage(5);
            flow.Trigger(msg);

            Assert.AreEqual(100, msg.Value);
            Assert.AreEqual(5, msg.GetValue<int>("test"));
        }

        public class PowerShellRuntimeProvider : IScriptRuntimeProvider
        {
            public string ScriptLanguage => "ps1";
            public IScriptRuntime CreateRuntime() => new PowerShellRuntime();
        }

        public class PowerShellRuntime : IScriptRuntime, IDisposable
        {
            PowerShell _powerShell;

            public PowerShellRuntime()
            {
                _powerShell = PowerShell.Create();
            }

            public void BeginInvoke<T>(Action<T[]> callback)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                _powerShell?.Dispose();
            }

            public object GetVariable(string name)
            {
                return _powerShell.Runspace.SessionStateProxy.GetVariable(name);
            }

            public T GetVariable<T>(string name)
            {
                return (T)GetVariable(name);
            }

            public T[] Invoke<T>()
            {
                return _powerShell.Invoke<T>().ToArray();
            }

            public void LoadFile(string path)
            {
                var script = System.IO.File.ReadAllText(path);
                LoadScript(script);
            }

            public void LoadScript(string script)
            {
                _powerShell.AddScript(script);
            }

            public void SetVariable(string name, object value)
            {
                _powerShell.Runspace.SessionStateProxy.SetVariable(name, value);
            }
        }

        public class Schweinbroon
        {
            private readonly Action<int> _invoke;

            public string Title => "Schweinbroon";

            public Schweinbroon(Action<int> invoke)
            {
                _invoke = invoke;
            }

            public void MakeOnConsole(int value)
            {
                _invoke(value);
            }
        }
    }
}
