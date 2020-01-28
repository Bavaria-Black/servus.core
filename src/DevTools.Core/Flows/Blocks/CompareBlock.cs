using System;

namespace DevTools.Core.Flows.Blocks
{
    public class CompareBlock : BlockBase
    {
        private readonly Func<Message, bool> _comparer;

        public CompareBlock(Func<Message, bool> comparer) 
            : base(2)
        {
            _comparer = comparer;
        }

        protected override Message[] Run(Message input)
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
