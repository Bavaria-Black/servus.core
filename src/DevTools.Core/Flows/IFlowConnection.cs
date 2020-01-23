using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows
{
    public interface IFlowConnection
    {
        void Trigger(MessageBase message);
    }
}
