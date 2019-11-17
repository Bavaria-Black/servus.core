using DevTools.Core.Flows.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows
{
    public class LogicConnection<T> : ILogicConnection<T> where T : MessageBase, new()
    {
        private readonly LogicBlock<T> _target;

        public int SourceOutput { get; }
        public Guid SourceId { get; }
        public Guid TargetId => _target.InstanceId;

        public LogicConnection(LogicBlock<T> source, int sourceOutput, LogicBlock<T> target)
        {
            SourceOutput = sourceOutput;
            SourceId = source.InstanceId;
            source.Connect(this, sourceOutput);
            _target = target;
        }

        public void Trigger(T message)
        {
            _target.TriggerInput(message);
        }
    }
}
