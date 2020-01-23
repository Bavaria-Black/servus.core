using DevTools.Core.Serialization;
using System.IO;

namespace DevTools.Core.Flows
{
    public static class FlowFactory
    {
        public static Flow Create()
        {
            return new Flow();
        }

        public static string Serialize(ISerializer serializer, Flow value)
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

        public static void Serialize(ISerializer serializer, StreamWriter writer, Flow value)
        {
            serializer.Serialize(writer, value);
        }

        public static Flow Deserialize(ISerializer serializer, string data)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, System.Text.Encoding.UTF8, 1024, true))
            using (var reader = new StreamReader(stream))
            {
                writer.Write(data);
                writer.Flush();

                stream.Position = 0;
                return Deserialize(serializer, reader);
            }
        }

        public static Flow Deserialize(ISerializer serializer, StreamReader reader)
        {
            var flow = serializer.Deserialize<Flow>(reader);
            flow.Init();

            return flow;
        }
    }
}
