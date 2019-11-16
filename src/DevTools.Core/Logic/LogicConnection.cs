using DevTools.Core.Logic.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Logic
{
    public class LogicConnection<T> : ILogicConnection<T> where T : MessageBase, new()
    {
        private readonly LogicBlock<T> _target;

        public LogicConnection(LogicBlock<T> source, int sourceOutput, LogicBlock<T> target)
        {
            source.Connect(this, sourceOutput);
            _target = target;
        }

        public void Trigger(T message)
        {
            _target.TriggerInput(message);
        }
    }
}
