using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows.Blocks
{
    public class DebugBlock : LogicBlock<IntMessage>
    {

        public DebugBlock() 
            : base(1)
        {
        }

        protected override IntMessage[] Run(IntMessage input)
        {
            Console.WriteLine(input.Value);
            return new[] { input };
        }
    }
}
