namespace DevTools.Core.Flows.Blocks
{
    public class AddConstantBlock : BlockBase
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

        protected override Message[] Run(Message input)
        {
            var value = input.GetValue<int>("Value");
            value += ConstantValue;
            input.SetValue("Value", value);
            return new[] { input };
        }
    }
}
