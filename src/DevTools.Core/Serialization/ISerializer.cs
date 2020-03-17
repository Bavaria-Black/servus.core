using System.IO;

namespace DevTools.Core.Serialization
{
    public interface ISerializer
    {
        string Serialize(object value);
        void Serialize(object value, StreamWriter writer);

        T Deserialze<T>(string value);
        T Deserialze<T>(StreamReader reader);
        object Deserialze(StreamReader reader);
    }
}
