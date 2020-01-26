using System;

namespace DevTools.Core.Flows.Blocks
{
    public class CompareBlock : BlockBase
    {
        private readonly Func<MessageBase, bool> _comparer;

        public CompareBlock(Func<MessageBase, bool> comparer) 
            : base(2)
        {
            _comparer = comparer;
        }

        protected override MessageBase[] Run(MessageBase input)
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
