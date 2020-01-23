using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows.Blocks
{
    public class FunctionBlock : BlockBase
    {
        private readonly Func<MessageBase, MessageBase[]> _function;

        public FunctionBlock(int outputCount, Func<MessageBase, MessageBase[]> function)
            : base(outputCount)
        {
            _function = function;
        }

        protected override MessageBase[] Run(MessageBase input) => _function(input);
    }
}
