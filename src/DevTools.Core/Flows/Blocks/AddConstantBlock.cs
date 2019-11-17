using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows.Blocks
{
    public class AddConstantBlock : LogicBlock<IntMessage>
    {
        public int ConstantValue { get; set; }

        public AddConstantBlock()
            : base(1)
        {

        }

        public AddConstantBlock(int constantValue)
            : this()
        {
            ConstantValue = constantValue;
        }

        protected override IntMessage[] Run(IntMessage input)
        {
            input.Value += ConstantValue;
            return new[] { input };
        }
    }
}
