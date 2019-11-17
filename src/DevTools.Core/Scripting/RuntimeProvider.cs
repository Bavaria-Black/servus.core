using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Scripting
{
    public interface IScriptRuntimeProvider
    {
        string ScriptLanguage { get; }
        IScriptRuntime CreateRuntime();
    }
}
