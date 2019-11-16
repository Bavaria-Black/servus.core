using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Logic
{
    public interface ILogicConnection<T> where T : MessageBase
    {
        void Trigger(T message);
    }
}
