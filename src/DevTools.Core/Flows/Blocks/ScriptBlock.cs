using System;
using System.Collections.Generic;

namespace DevTools.Core.Flows.Blocks
{
    public class ScriptBlock : BlockBase
    {
        public string Script { get; set; }

        public ScriptBlock(int outputCount, string script)
            : base(outputCount)
        {
            Script = script;
        }

        protected override MessageBase[] Run(MessageBase input)
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
                    var outputMessages = new List<MessageBase>();
                    foreach(var element in array)
                    {
                        if(element is MessageBase message)
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
