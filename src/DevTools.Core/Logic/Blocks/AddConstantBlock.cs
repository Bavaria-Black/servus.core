using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Logic.Blocks
{
    public class AddConstantBlock : LogicBlock<IntMessage>
    {
        public int ConstantValue { get; }

        public AddConstantBlock(int constantValue)
            : base(1)
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
