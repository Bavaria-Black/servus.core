using DevTools.Core.Scripting;
using DevTools.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows
{
    public interface IFlowContext
    {
        void RegisterScriptRuntimeProvider(IScriptRuntimeProvider runtimeProvider);
        IScriptRuntime GetScriptRuntime(string scriptLanguage);

        T GetValue<T>(string key);
        void SetValue(string key, object value);
    }
}
