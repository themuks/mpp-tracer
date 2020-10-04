namespace tracer
{
    public interface ITracer
    {
        void StartTrace();
        void StopTrace();
        TracingThread[] GetTraceResult();
    }
}