using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows.Blocks
{
    public class ScriptBlock<T> : LogicBlock<T> where T : MessageBase, new()
    {
        public string Script { get; set; }

        public ScriptBlock(int outputCount, string script)
            : base(outputCount)
        {
            Script = script;
        }

        protected override T[] Run(T input)
        {
            var runtime = Context.GetScriptRuntime("ps1");

            try
            {
                runtime.LoadScript(Script);
                runtime.SetVariable("msg", input);

                // think about a solution for debugging the powershell script output
                var result = runtime.Invoke<object>();

                var output = runtime.GetVariable("output");
                if (output != null && output is Array array)
                {
                    List<T> outputMessages = new List<T>();
                    foreach(var element in array)
                    {
                        if(element is T message)
                        {
                            outputMessages.Add(message);
                        }
                        else if (element == null)
                        {
                            outputMessages.Add(null);
                        }
                    }

                    if(outputMessages.Count > 0)
                    {
                        return outputMessages.ToArray();
                    }
                }
            }
            finally
            {
                if (runtime is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            return new[] {
                input
            };
        }
    }
}
