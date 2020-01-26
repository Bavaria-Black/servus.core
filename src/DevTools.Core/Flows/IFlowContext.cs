using DevTools.Core.Scripting;

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
