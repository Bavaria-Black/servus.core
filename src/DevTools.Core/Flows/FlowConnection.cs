using DevTools.Core.Flows.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows
{
    public class FlowConnection : IFlowConnection
    {
        private readonly BlockBase _target;

        public int SourceOutput { get; }
        public Guid SourceId { get; }
        public Guid TargetId => _target.InstanceId;

        public FlowConnection(BlockBase source, int sourceOutput, BlockBase target)
        {
            SourceOutput = sourceOutput;
            SourceId = source.InstanceId;
            source.Connect(this, sourceOutput);
            _target = target;
        }

        public void Trigger(MessageBase message)
        {
            _target.TriggerInput(message);
        }
    }
}
