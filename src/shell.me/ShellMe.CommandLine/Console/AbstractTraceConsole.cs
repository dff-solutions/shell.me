using System.Diagnostics;

namespace ShellMe.CommandLine.Console
{
    public abstract class AbstractTraceConsole : AbstractConsole
    {
        public abstract void TraceEvent(TraceEventType traceEventType, int code, string message);
    }
}
