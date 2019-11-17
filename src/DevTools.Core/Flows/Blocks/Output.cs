using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevTools.Core.Flows.Blocks
{
    internal class Output<T> where T : MessageBase
    {
        private readonly List<ILogicConnection<T>> _connetions = new List<ILogicConnection<T>>();
        internal void Add(ILogicConnection<T> connection)
        {
            _connetions.Add(connection);
        }

        internal void Trigger(T message)
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