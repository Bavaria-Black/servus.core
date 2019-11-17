using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DevTools.Core.Flows.Blocks
{
    public abstract class LogicBlock<T> where T : MessageBase, new()
    {
        public Guid InstanceId { get; set; } = Guid.NewGuid();

        [IgnoreDataMember]
        public IFlowContext Context { get; internal set; }

        private readonly Dictionary<int, Output<T>> _outputs = new Dictionary<int, Output<T>>();

        public LogicBlock(int outputCount)
        {
            for (int i = 0; i < outputCount; i++)
            {
                _outputs.Add(i, new Output<T>());
            }
        }

        public void TriggerInput(T input)
        {
            try
            {
                var a = input.Duplicate<T>();
                var outputmessages = Run(input);
                var count = Math.Max(outputmessages.Length, _outputs.Count);
                for (int i = 0; i < count; i++)
                {
                    var message = outputmessages[i];
                    if (message != default(T))
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

        protected abstract T[] Run(T input);

        private void TriggerOutput(int output, T message)
        {
            _outputs[output].Trigger(message);
        }

        internal void Connect(ILogicConnection<T> connection, int output)
        {
            _outputs[output].Add(connection);
        }
    }
}
