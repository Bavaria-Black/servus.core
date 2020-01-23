using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows.Blocks
{
    public class DebugBlock : BlockBase
    {

        public DebugBlock() 
            : base(1)
        {
        }

        protected override MessageBase[] Run(MessageBase input)
        {
            Console.WriteLine(input.GetValue<object>("Value"));
            return new[] { input };
        }
    }
}
