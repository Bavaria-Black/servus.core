using DevTools.Core.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevTools.Core.Tests.Serialization
{
    public class JsonSerializer : ISerializer
    {
        private Newtonsoft.Json.JsonSerializer _serializer;

        public JsonSerializer()
        {
            _serializer = new Newtonsoft.Json.JsonSerializer();
            _serializer.TypeNameHandling = TypeNameHandling.Auto;
        }

        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public void Serialize(TextWriter textWriter, object value)
        {
            using (var writer = new JsonTextWriter(textWriter))
            {
                _serializer.Serialize(writer, value);
            }
        }

        public T Deserialize<T>(TextReader textReader)
        {
            using (var reader = new JsonTextReader(textReader))
            {
                return _serializer.Deserialize<T>(reader);
            }
        }

        public object Deserialize(TextReader textReader, Type targetType)
        {
            using (var reader = new JsonTextReader(textReader))
            {
                return _serializer.Deserialize(reader, targetType);
            }
        }
    }
}
