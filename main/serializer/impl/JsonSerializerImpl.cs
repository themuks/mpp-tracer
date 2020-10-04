using System.IO;
using System.Text;
using System.Text.Json;
using tracer;

namespace mpp_tracer
{
    public class JsonSerializerImpl : ISerializer
    {
        public byte[] Serialize(object o)
        {
            JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string serializedData = JsonSerializer.Serialize(o, serializerOptions);
            return Encoding.UTF8.GetBytes(serializedData);
        }
    }
}