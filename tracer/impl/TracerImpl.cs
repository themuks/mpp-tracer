using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace tracer
{
    public class TracerImpl : ITracer
    {
        private const int FirstFrameIndex = 1;
        
        private ConcurrentDictionary<int, TracingThread> _threadDictionary = new ConcurrentDictionary<int, TracingThread>();

        public void StartTrace()
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            if (!_threadDictionary.ContainsKey(currentThreadId))
            {
                TracingThread thread = new TracingThread(currentThreadId);
                _threadDictionary.TryAdd(currentThreadId, thread);
            }        
            MethodBase currentMethod = new StackTrace().GetFrame(FirstFrameIndex).GetMethod();
            TraceResult traceResult = new TraceResult(currentMethod.Name, currentMethod.DeclaringType.Name);
            _threadDictionary[currentThreadId].BeginMethodTrace(traceResult);
        }

        public void StopTrace()
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            _threadDictionary[currentThreadId].StopMethodTrace();
        }

        public TracingThread[] GetTraceResult()
        {
            TracingThread[] threads = _threadDictionary.Values.ToArray();
            foreach (TracingThread thread in threads)
            {
                thread.CalculateThreadElapsedTime();
            }
            return threads;
        }
    }
}