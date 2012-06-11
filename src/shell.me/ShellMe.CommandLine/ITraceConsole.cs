using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine
{
    public interface ITraceConsole : IConsole
    {
        void TraceEvent(TraceEventType traceEventType, int code, string message);
    }
}
