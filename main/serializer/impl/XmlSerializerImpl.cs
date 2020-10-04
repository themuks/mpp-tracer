using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using tracer;

namespace mpp_tracer
{
    public class XmlSerializer : ISerializer
    {
        public byte[] Serialize(TracingThread[] threads)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(TracingThread[]));
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true
            };
            MemoryStream memoryStream = new MemoryStream();
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
            {
                serializer.WriteObject(xmlWriter, threads);
            }
            return memoryStream.GetBuffer();
        }
    }
}