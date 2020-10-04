using System;
using System.Text;

namespace mpp_tracer
{
    public class ConsoleOutputImpl : IOutput
    {
        public void writeData(byte[] data)
        {
            string stringData = Encoding.UTF8.GetString(data);
            Console.WriteLine(stringData);
        }
    }
}