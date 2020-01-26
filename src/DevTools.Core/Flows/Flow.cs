using DevTools.Core.Flows.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevTools.Core.Flows
{
    public class Flow
    {
        public class SerializableConnection
        {
            public Guid SourceId { get; set; }
            public int SourceOutput { get; set; }
            public Guid TargetId { get; set; }

            public SerializableConnection()
            {

            }

            public SerializableConnection(Guid sourceId, int sourceOutput, Guid targetId)
            {
                SourceId = sourceId;
                SourceOutput = sourceOutput;
                TargetId = targetId;
            }
        }

        public IFlowContext Context = new FlowContext();

        private readonly List<FlowConnection> _connections  = new List<FlowConnection>();

        public List<BlockBase> Blocks { get; set; } = new List<BlockBase>();
        public List<SerializableConnection> Connections { get; set; } = new List<SerializableConnection>();
        public List<BlockBase> EntryBlocks { get; set; } = new List<BlockBase>();

        internal Flow()
        {
        }

        internal void Init()
        {
            foreach(var block in Blocks)
            {
                block.Context = Context;
            }

            foreach(var info in Connections)
            {
                var source = Blocks.First(b => b.InstanceId == info.SourceId);
                var target = Blocks.First(b => b.InstanceId == info.TargetId);
                var connection = new FlowConnection(source, info.SourceOutput, target);
                _connections.Add(connection);
            }
        }

        public void AddBlock(BlockBase block)
        {
            block.Context = Context;
            Blocks.Add(block);
        }

        public void AddConnection(BlockBase source, int sourceOutput, BlockBase target)
        {
            var connection = new FlowConnection(source, sourceOutput, target);
            AddConnection(connection);
        }

        public void AddConnection(FlowConnection connection)
        {
            Connections.Add(new SerializableConnection(connection.SourceId, connection.SourceOutput, connection.TargetId));
            _connections.Add(connection);
        }

        public void Trigger(MessageBase msg)
        {
            Parallel.ForEach(EntryBlocks, (b) =>
            {
                try
                {
                    b.TriggerInput(msg);
                }
                catch
                {
                    // think about a solution
                }
            });
        }

        public void AddEntryBlock(BlockBase block)
        {
            EntryBlocks.Add(block);
        }
    }
}
