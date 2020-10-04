using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace tracer
{
    [System.Runtime.Serialization.DataContract]
    [Serializable]
    public class TraceResult
    {
        [JsonPropertyName("name")]
        [System.Runtime.Serialization.DataMember(Name = "name")]
        public string MethodName { get; private set; }

        [JsonPropertyName("class")]
        [System.Runtime.Serialization.DataMember(Name = "class")]
        public string ClassName { get; private set; }

        [JsonPropertyName("time")]
        [System.Runtime.Serialization.DataMember(Name = "time")]
        public long Time { get; private set; }

        [JsonPropertyName("methods")]
        [System.Runtime.Serialization.DataMember(Name = "methods")]
        public List<TraceResult> Methods { get; private set; } = new List<TraceResult>();
        
        private Stopwatch _stopwatch = new Stopwatch();
        
        public TraceResult(string methodName, string className)
        {
            MethodName = methodName;
            ClassName = className;
        }

        public void ExecutionStart()
        {
            _stopwatch.Start();
        }
        public void ExecutionFinished()
        {
            _stopwatch.Stop();
            Time = _stopwatch.ElapsedMilliseconds;
        }
    }
}