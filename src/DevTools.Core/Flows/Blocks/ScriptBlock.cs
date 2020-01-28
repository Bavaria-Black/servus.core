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

        protected override Message[] Run(Message input)
        {
            var runtime = Context.GetScriptRuntime("ps1");

            try
            {
                runtime.LoadScript(Script);
                runtime.SetVariable("msg", input);

                var result = runtime.Invoke<object>();
                foreach(var line in result)
                {
                    // think about a better solution for debugging the powershell script output
                    Console.WriteLine(line);
                }

                if (runtime.GetVariable("output") is Array array)
                {
                    var outputMessages = new List<Message>();
                    foreach(var element in array)
                    {
                        if(element is Message message)
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
