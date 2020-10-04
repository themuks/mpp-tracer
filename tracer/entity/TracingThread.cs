using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading;

namespace tracer
{
    [System.Runtime.Serialization.DataContract(Name = "thread")]
    [Serializable]
    public class TracingThread
    {
        private Stopwatch _stopwatch = new Stopwatch();

        [JsonPropertyName("id")]
        [System.Runtime.Serialization.DataMember(Name = "id")]
        public int ThreadId { get; private set; }

        [JsonPropertyName("time")]
        [System.Runtime.Serialization.DataMember(Name = "time")]
        public long ThreadTimeElapsed { get; private set; }

        [JsonPropertyName("methods")]
        [System.Runtime.Serialization.DataMember(Name = "methods")]
        public List<TraceResult> MethodList { get; private set; } = new List<TraceResult>();

        private Stack<TraceResult> _methodStack = new Stack<TraceResult>();

        public TracingThread(int threadId)
        {
            ThreadId = threadId;
        }
        
        public void BeginMethodTrace(TraceResult result)
        {
            result.ExecutionStart();
            if (_methodStack.Count == 0)
            {
                MethodList.Add(result);               
            }
            else
            {
                _methodStack.Peek().Methods.Add(result);
            }
            _methodStack.Push(result);
        }

        public void StopMethodTrace()
        {
            TraceResult traceResult = _methodStack.Pop();
            traceResult.ExecutionFinished();   
        }

        public void CalculateThreadElapsedTime()
        {
            ThreadTimeElapsed = 0;
            foreach(TraceResult traceResult in MethodList)
            {
                ThreadTimeElapsed += traceResult.Time;
            }
        }
    }
}