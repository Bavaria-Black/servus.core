using System;
using System.IO;

namespace DevTools.Core.Serialization
{
    public interface ISerializer
    {
        string Serialize(object value);
        void Serialize(TextWriter stream, object value);

        T Deserialize<T>(TextReader stream);
        object Deserialize(TextReader stream, Type targetType);
    }
}
