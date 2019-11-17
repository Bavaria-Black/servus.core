using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows
{
    public interface ILogicConnection<T> where T : MessageBase
    {
        void Trigger(T message);
    }
}
