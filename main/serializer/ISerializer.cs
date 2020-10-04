using System.IO;
using tracer;

namespace mpp_tracer
{
    public interface ISerializer
    {
        byte[] Serialize(TracingThread[] threads);
    }
}