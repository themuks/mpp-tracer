using System;
using System.Threading;
using tracer;

namespace mpp_tracer
{
    static class StaticTracer
    {
        public static readonly ITracer Tracer = new TracerImpl();
    }
    class TestClass1
    {
        public void PrintFunc(string text, int count)
        {
            StaticTracer.Tracer.StartTrace();
            Thread.Sleep(5);
            for (int i = 0; i < count; i++)
                Console.WriteLine(text);
            StaticTracer.Tracer.StopTrace();
        }

        public void NestedFunc(int callTimes)
        {
            StaticTracer.Tracer.StartTrace();
            Thread.Sleep(5);
            Console.WriteLine($"NestedFunc was called, {callTimes} calls left");
            if (callTimes != 0)
            {
                NestedFunc(callTimes - 1);
            }
            Console.WriteLine($"NestedFunc that had {callTimes} calls left is returning");
            StaticTracer.Tracer.StopTrace();
        }

        public void CascadeFunc(int callTimes)
        {
            StaticTracer.Tracer.StartTrace();
            Thread.Sleep(5);
            Console.WriteLine($"CascadeFunc was called, {callTimes} calls left");
            if (callTimes != 0)
            {
                CascadeFunc(callTimes - 1);
                CascadeFunc(callTimes - 1);
            }            
            StaticTracer.Tracer.StopTrace();
        }
    }

    class TestClass2
    {
        public void MultiClassFunc()
        {
            StaticTracer.Tracer.StartTrace();
            Thread.Sleep(5);
            TestClass1 testClass1 = new TestClass1();
            for (int i = 0; i < 3; i++)
            {
                testClass1.PrintFunc($"MultiClassFunc called print func with arg {i}", i);
            }
            testClass1.NestedFunc(2);
            testClass1.CascadeFunc(2);
            StaticTracer.Tracer.StopTrace();
        }
    }

    class TestClass3
    {
        public void Method1()
        {
            StaticTracer.Tracer.StartTrace();
            Thread.Sleep(50);
            StaticTracer.Tracer.StopTrace();
        }
        
        public void Method2()
        {
            StaticTracer.Tracer.StartTrace();
            Thread.Sleep(50);
            StaticTracer.Tracer.StopTrace();
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Thread thread1 = new Thread(() => {
                StaticTracer.Tracer.StartTrace();
                TestClass2 testClass2 = new TestClass2();
                testClass2.MultiClassFunc();
                StaticTracer.Tracer.StopTrace();
            });
            thread1.Start();
            Thread thread2 = new Thread(() =>
            {
                StaticTracer.Tracer.StartTrace();
                TestClass3 testClass3 = new TestClass3();
                testClass3.Method1();
                testClass3.Method2();
                StaticTracer.Tracer.StopTrace();
            });
            thread2.Start();
            Thread thread3 = new Thread(() =>
            {
                TestClass1 testClass1 = new TestClass1();
                testClass1.PrintFunc("text", 1);
                testClass1.PrintFunc("text", 1);
                testClass1.PrintFunc("text", 1);
                testClass1.PrintFunc("text", 1);
            });
            thread3.Start();
            thread1.Join();
            thread2.Join();
            thread3.Join();
            TracingThread[] threads = StaticTracer.Tracer.GetTraceResult();
            ISerializer serializer = new XmlSerializer();
            byte[] bytesFromXml = serializer.Serialize(threads);
            serializer = new JsonSerializerImpl();
            byte[] bytesFromJson = serializer.Serialize(threads);
            IOutput output = new ConsoleOutputImpl();
            output.writeData(bytesFromJson);
            output.writeData(bytesFromXml);
            output = new FileOutputImpl("C:\\Users\\Admin\\Desktop\\testFileJson.txt");
            output.writeData(bytesFromJson);
            output = new FileOutputImpl("C:\\Users\\Admin\\Desktop\\testFileXml.txt");
            output.writeData(bytesFromXml);
        }
    }
}