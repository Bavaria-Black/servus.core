namespace DevTools.Core.Flows
{
    public class IntMessage : Message
    {
        public int Value {
            get => GetValue<int>(nameof(Value));
            set => SetValue(nameof(Value), value);
        }

        public IntMessage() 
            : this(0)
        {

        }

        public IntMessage(int value)
        {
            Value = value;
        }
    }
}
