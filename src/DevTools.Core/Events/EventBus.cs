using System;

namespace DevTools.Core.Events
{
    //ToDo: Is the folder events alright?
    //todo:  Publish T?
    //todo: Subscribe to multiple T on same topic
    public class EventBus
    {
        
        
        public void Publish<T>(string topic, T message)
        {
            
        }

        public void Subscribe<T>(Action<T> action)
        {
            
        }
    }
}