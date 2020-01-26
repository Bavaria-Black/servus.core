namespace DevTools.Core.Flows
{
    public interface IFlowConnection
    {
        void Trigger(MessageBase message);
    }
}
