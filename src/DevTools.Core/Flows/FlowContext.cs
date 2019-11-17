using System;
using System.Collections.Generic;
using System.Text;
using DevTools.Core.Scripting;

namespace DevTools.Core.Flows
{
    internal class FlowContext : IFlowContext
    {
        private Dictionary<string, IScriptRuntimeProvider> _scriptRuntimeProvider = new Dictionary<string, IScriptRuntimeProvider>();
        public Dictionary<string, object> ValueStore { get; set; } = new Dictionary<string, object>();

        public IScriptRuntime GetScriptRuntime(string scriptLanguage)
        {
            return _scriptRuntimeProvider[scriptLanguage].CreateRuntime();
        }

        public T GetValue<T>(string key) => (T)ValueStore[key];
        public void SetValue(string key, object value) => ValueStore.Add(key, value);

        public void RegisterScriptRuntimeProvider(IScriptRuntimeProvider runtimeProvider)
        {
            _scriptRuntimeProvider.Add(runtimeProvider.ScriptLanguage, runtimeProvider);
        }
    }
}
