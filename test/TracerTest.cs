using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tracer;

namespace test
{
    [TestClass]
    public class TracerTest
    {
        private const int SleepTime = 20;
        private static ITracer _tracer;

        [TestInitialize]
        public void TestInit()
        {
            _tracer = new TracerImpl();            
        }

        private static class SimpleTest {
            public static void SimpleCall()
            {
                _tracer.StartTrace();
                Thread.Sleep(SleepTime);
                _tracer.StopTrace();
            }
        }

        private static class SequencedTest
        {
            public static void SimpleSequenceCall()
            {
                SimpleTest.SimpleCall();
                SimpleTest.SimpleCall();
                SimpleTest.SimpleCall();
            }

            public static void NestedSequenceCall()
            {
                _tracer.StartTrace();
                SimpleSequenceCall();
                _tracer.StopTrace();
            }
        }

        private static class NestedTest
        {
            public static void NestedCall()
            {
                _tracer.StartTrace();
                SimpleTest.SimpleCall();
                _tracer.StopTrace();
            }
        }

        private static class RecursionTest
        {
            public static void RecursiveCall(int count)
            {
                _tracer.StartTrace();
                if (count != 0)
                {
                    RecursiveCall(count-1);                   
                }
                Thread.Sleep(SleepTime);
                _tracer.StopTrace();
            }

            public static void CascadeCall(int count)
            {
                _tracer.StartTrace();
                if (count != 0)
                {
                    CascadeCall(count - 1);
                    CascadeCall(count - 1);                   
                }
                Thread.Sleep(SleepTime);
                _tracer.StopTrace();
            }
            
            private static void MutualCall2(int count)
            {
                _tracer.StartTrace();
                if (count != 0)
                {
                    MutualCall(count - 1);                    
                }
                Thread.Sleep(SleepTime);
                _tracer.StopTrace();
            }

            public static void MutualCall(int count)
            {
                _tracer.StartTrace();
                if (count != 0)
                {
                    MutualCall2(count);                   
                }
                Thread.Sleep(SleepTime);
                _tracer.StopTrace();
            }
        }

        private static class OverloadTest
        {
            private static class OverloadClass
            {
                public static void Overload(int arg)
                {
                    _tracer.StartTrace();
                    Thread.Sleep(SleepTime);
                    _tracer.StopTrace();
                }

                public static void Overload(char arg)
                {
                    _tracer.StartTrace();
                    Thread.Sleep(SleepTime);
                    _tracer.StopTrace();
                }
            }

            private static void Overload(int arg)
            {
                _tracer.StartTrace();
                OverloadClass.Overload('b');
                Thread.Sleep(SleepTime);
                _tracer.StopTrace();

            }
            private static void Overload(char arg)
            {
                _tracer.StartTrace();
                OverloadClass.Overload(2);
                Thread.Sleep(SleepTime);
                _tracer.StopTrace();

            }
            
            public static void OverloadCall()
            {
                _tracer.StartTrace();
                Overload(1);
                Overload('a');
                Thread.Sleep(SleepTime);
                _tracer.StopTrace();
            }

        }

        private void Equals(TraceResult expected, TraceResult actual)
        {
            Assert.AreEqual(expected.MethodName, actual.MethodName);
            Assert.AreEqual(expected.ClassName, actual.ClassName);
            Assert.AreEqual(expected.Methods.Count, actual.Methods.Count);
            Assert.IsNotNull(actual.Time);
        }

        [TestMethod]
        public void MeasureSimpleCall_None_Equal()
        {
            SimpleTest.SimpleCall();
            var actual = _tracer.GetTraceResult()[0].MethodList[0];
            var expected = new TraceResult("SimpleCall", "SimpleTest");
            Equals(actual, expected);          
        }

        [TestMethod]
        public void MeasureNestedCall_None_Equal()
        {
            NestedTest.NestedCall();
            var actual = _tracer.GetTraceResult()[0].MethodList[0].Methods[0];
            var expected = new TraceResult("SimpleCall", "SimpleTest");
            Equals(actual, expected);
        }

        [TestMethod]
        public void RecursiveCall_2_Equal()
        {
            RecursionTest.RecursiveCall(2);
            var actual = _tracer.GetTraceResult()[0].MethodList[0].Methods[0].Methods[0];
            var expected = new TraceResult("RecursiveCall", "RecursionTest");
            Equals(actual, expected);
        }

        [TestMethod]
        public void CascadeCall_1_Equal()
        {
            RecursionTest.CascadeCall(1);
            var actual = _tracer.GetTraceResult()[0].MethodList[0].Methods[1];
            var expected = new TraceResult("CascadeCall", "RecursionTest");
            Equals(actual, expected);
        }

        [TestMethod]
        public void MutualRecursionCall_1_Equal()
        {
            RecursionTest.MutualCall(1);
            var actual1 = _tracer.GetTraceResult()[0].MethodList[0].Methods[0];
            var actual2 = _tracer.GetTraceResult()[0].MethodList[0].Methods[0].Methods[0];
            var expected1 = new TraceResult("MutualCall2", "RecursionTest");
            expected1.Methods.Add(new TraceResult("MutualCall", "RecursionTest"));
            var expected2 = new TraceResult("MutualCall", "RecursionTest");
            Equals(actual1, expected1);
            Equals(actual2, expected2);
        }

        [TestMethod]
        public void SimpleSequencedCall_None_Equal()
        {
            SequencedTest.SimpleSequenceCall();
            var actual = _tracer.GetTraceResult()[0].MethodList[0].Methods;
            var expected = new TraceResult("SimpleCall", "SequencedTest");
            expected.Methods.Add(new TraceResult("SimpleCall","SimpleTest"));
            expected.Methods.Add(new TraceResult("SimpleCall", "SimpleTest"));
            expected.Methods.Add(new TraceResult("SimpleCall", "SimpleTest"));
            Equals(actual, expected);
        }

        [TestMethod]
        public void NestedSequencedCall_None_Equal()
        {
            SequencedTest.NestedSequenceCall();
            var actual = _tracer.GetTraceResult()[0].MethodList[0];
            var expected = new TraceResult("NestedSequenceCall", "SequencedTest");
            expected.Methods.Add(new TraceResult("SimpleCall", "SimpleTest"));
            expected.Methods.Add(new TraceResult("SimpleCall", "SimpleTest"));
            expected.Methods.Add(new TraceResult("SimpleCall", "SimpleTest"));
            Equals(actual, expected);
        }

        [TestMethod]
        public void OverloadCall_None_Equal()
        {
            OverloadTest.OverloadCall();
            var actualMain = _tracer.GetTraceResult()[0].MethodList[0];
            var expectedMain = new TraceResult("OverloadCall", "OverloadTest");
            var actual1 = _tracer.GetTraceResult()[0].MethodList[0].Methods[0];
            var expected1 = new TraceResult("Overload", "OverloadTest");
            var actual2 = _tracer.GetTraceResult()[0].MethodList[0].Methods[1];
            var expected2 = new TraceResult("Overload", "OverloadTest");
            var actual3 = _tracer.GetTraceResult()[0].MethodList[0].Methods[0].Methods[0];
            var expected3 = new TraceResult("Overload", "OverloadClass");
            var actual4 = _tracer.GetTraceResult()[0].MethodList[0].Methods[1].Methods[0];
            var expected4 = new TraceResult("Overload", "OverloadClass");

            expectedMain.Methods.Add(expected1);
            expectedMain.Methods.Add(expected2);
            expected1.Methods.Add(expected3);
            expected2.Methods.Add(expected4);
            
            Equals(actualMain, expectedMain);
            Equals(actual1, expected1);
            Equals(actual2, expected2);
            Equals(actual3, expected3);
            Equals(actual4, expected4);
        }
    }
}