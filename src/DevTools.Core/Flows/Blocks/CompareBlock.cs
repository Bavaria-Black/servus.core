using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows.Blocks
{
    public class CompareBlock<T> : LogicBlock<T> where T : MessageBase, new()
    {
        private readonly Func<T, bool> _comparer;

        public CompareBlock(Func<T, bool> comparer) 
            : base(2)
        {
            _comparer = comparer;
        }

        protected override T[] Run(T input)
        {
            if(_comparer(input))
            {
                return new[] { input, default };
            }
            else
            {
                return new[] { default, input};
            }
        }
    }
}
