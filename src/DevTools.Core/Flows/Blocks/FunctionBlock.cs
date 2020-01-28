using System;

namespace DevTools.Core.Flows.Blocks
{
    public class FunctionBlock : BlockBase
    {
        private readonly Func<Message, Message[]> _function;

        public FunctionBlock(int outputCount, Func<Message, Message[]> function)
            : base(outputCount)
        {
            _function = function;
        }

        protected override Message[] Run(Message input) => _function(input);
    }
}
