using System;

namespace DevTools.Core.Flows.Blocks
{
    public class DebugBlock : BlockBase
    {

        public DebugBlock() 
            : base(1)
        {
        }

        protected override Message[] Run(Message input)
        {
            Console.WriteLine(input.GetValue<object>("Value"));
            return new[] { input };
        }
    }
}
