using System.Collections.Generic;

namespace DevTools.Core.Flows
{
    public class Message
    {
        private Dictionary<string, object> _valueStore = new Dictionary<string, object>();

        public T GetValue<T>(string key)
        {
            if(_valueStore.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return default;
        }

        public void SetValue<T>(string key, T value)
        {
            _valueStore[key] = value;
        }

        internal void SetValueStore(Dictionary<string, object> valueStore)
        {
            _valueStore = valueStore;
        }

        internal Message Duplicate()
        {
            var a = new Message();
            a.SetValueStore(new Dictionary<string, object>(_valueStore));
            return a;
        }
    }
}
