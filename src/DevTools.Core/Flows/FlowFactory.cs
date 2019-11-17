using DevTools.Core.Serialization;
using System.IO;

namespace DevTools.Core.Flows
{
    public static class FlowFactory
    {
        public static Flow<T> Create<T>() where T : MessageBase, new()
        {
            return new Flow<T>();
        }

        public static string Serialize<T>(ISerializer serializer, Flow<T> value) where T : MessageBase, new()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream, System.Text.Encoding.UTF8, 1024, true))
            {
                Serialize(serializer, writer, value);
                stream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        public static void Serialize<T>(ISerializer serializer, StreamWriter writer, Flow<T> value) where T : MessageBase, new()
        {
            serializer.Serialize(writer, value);
        }

        public static Flow<T> Deserialize<T>(ISerializer serializer, string data) where T : MessageBase, new()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, System.Text.Encoding.UTF8, 1024, true))
            using (var reader = new StreamReader(stream))
            {
                writer.Write(data);
                writer.Flush();

                stream.Position = 0;
                return Deserialize<T>(serializer, reader);
            }
        }

        public static Flow<T> Deserialize<T>(ISerializer serializer, StreamReader reader) where T : MessageBase, new()
        {
            var flow = serializer.Deserialize<Flow<T>>(reader);
            flow.Init();

            return flow;
        }
    }
}
