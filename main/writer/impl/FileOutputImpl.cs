using System.IO;

namespace mpp_tracer
{
    public class FileOutputImpl : IOutput
    {
        private string _filename;

        public FileOutputImpl(string filename)
        {
            this._filename = filename;
        }

        public void writeData(byte[] data)
        {
            File.WriteAllBytes(_filename, data);
        }
    }
}