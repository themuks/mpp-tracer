using System.IO;
using System.Text;
using System.Text.Json;
using tracer;

namespace mpp_tracer
{
    public class JsonSerializerImpl : ISerializer
    {
        public byte[] Serialize(TracingThread[] threads)
        {
            JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string serializedData = JsonSerializer.Serialize(threads, serializerOptions);
            return Encoding.UTF8.GetBytes(serializedData);
        }
    }
}