using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows.Blocks
{
    public class FunctionBlock<T> : LogicBlock<T> where T : MessageBase, new()
    {
        private readonly Func<T, T[]> _function;

        public FunctionBlock(int outputCount, Func<T, T[]> function)
            : base(outputCount)
        {
            _function = function;
        }

        protected override T[] Run(T input) => _function(input);
    }
}
