using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Flows
{
    public abstract class MessageBase
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

        internal T Duplicate<T>() where T : MessageBase, new()
        {
            var a = new T();
            a.SetValueStore(new Dictionary<string, object>(_valueStore));
            return a;
        }
    }
}
