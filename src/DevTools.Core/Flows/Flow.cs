using DevTools.Core.Flows.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Core.Flows
{
    public class Flow<T> where T : MessageBase, new()
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

        private readonly List<LogicConnection<T>> _connections  = new List<LogicConnection<T>>();

        public List<LogicBlock<T>> Blocks { get; set; } = new List<LogicBlock<T>>();
        public List<SerializableConnection> Connections { get; set; } = new List<SerializableConnection>();
        public List<LogicBlock<T>> EntryBlocks { get; set; } = new List<LogicBlock<T>>();

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
                var connection = new LogicConnection<T>(source, info.SourceOutput, target);
                _connections.Add(connection);
            }
        }

        public void AddBlock(LogicBlock<T> block)
        {
            block.Context = Context;
            Blocks.Add(block);
        }

        public void AddConnection(LogicBlock<T> source, int sourceOutput, LogicBlock<T> target)
        {
            var connection = new LogicConnection<T>(source, sourceOutput, target);
            AddConnection(connection);
        }

        public void AddConnection(LogicConnection<T> connection)
        {
            Connections.Add(new SerializableConnection(connection.SourceId, connection.SourceOutput, connection.TargetId));
            _connections.Add(connection);
        }

        public void Trigger(T msg)
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

        public void AddEntryBlock(LogicBlock<T> block)
        {
            EntryBlocks.Add(block);
        }
    }
}
