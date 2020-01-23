using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DevTools.Core.Flows.Blocks
{
    public abstract class BlockBase
    {
        public Guid InstanceId { get; set; } = Guid.NewGuid();

        [IgnoreDataMember]
        public IFlowContext Context { get; internal set; }

        private readonly Dictionary<int, Output> _outputs = new Dictionary<int, Output>();

        public BlockBase(int outputCount)
        {
            for (int i = 0; i < outputCount; i++)
            {
                _outputs.Add(i, new Output());
            }
        }

        public void TriggerInput(MessageBase input)
        {
            try
            {
                var a = input.Duplicate();
                var outputmessages = Run(input);
                var count = Math.Max(outputmessages.Length, _outputs.Count);
                for (int i = 0; i < count; i++)
                {
                    var message = outputmessages[i];
                    if (message != default)
                    {
                        TriggerOutput(i, message);
                    }
                }
            }
            catch (Exception ex)
            {
                // have to think about a solution
                Console.WriteLine(ex);
            }
        }

        protected abstract MessageBase[] Run(MessageBase input);

        private void TriggerOutput(int output, MessageBase message)
        {
            _outputs[output].Trigger(message);
        }

        internal void Connect(IFlowConnection connection, int output)
        {
            _outputs[output].Add(connection);
        }
    }
}
