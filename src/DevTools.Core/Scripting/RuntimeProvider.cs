namespace DevTools.Core.Scripting
{
    public interface IScriptRuntimeProvider
    {
        string ScriptLanguage { get; }
        IScriptRuntime CreateRuntime();
    }
}
