using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevTools.Core.Flows.Blocks
{
    internal class Output
    {
        private readonly List<IFlowConnection> _connections = new List<IFlowConnection>();
        internal void Add(IFlowConnection connection)
        {
            _connections.Add(connection);
        }

        internal void Trigger(Message message)
        {
            Parallel.ForEach(_connections, (c) =>
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