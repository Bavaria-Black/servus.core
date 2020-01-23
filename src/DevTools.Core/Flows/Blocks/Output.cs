using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevTools.Core.Flows.Blocks
{
    internal class Output
    {
        private readonly List<IFlowConnection> _connetions = new List<IFlowConnection>();
        internal void Add(IFlowConnection connection)
        {
            _connetions.Add(connection);
        }

        internal void Trigger(MessageBase message)
        {
            Parallel.ForEach(_connetions, (c) =>
            {
                try
                {
                    c.Trigger(message);
                }
                catch
                {
                    // have to think about a solution
                }
            });
        }
    }
}