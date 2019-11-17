using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Scripting
{
    public interface IScriptRuntime
    {
        void SetVariable(string name, object value);
        object GetVariable(string name);
        T GetVariable<T>(string name);
        void LoadFile(string path);
        void LoadScript(string script);
        T[] Invoke<T>();
        void BeginInvoke<T>(Action<T[]> callback);
    }
}
