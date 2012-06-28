using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine
{
    public abstract class AbstractTraceConsole : AbstractConsole
    {
        public abstract void TraceEvent(TraceEventType traceEventType, int code, string message);
    }
}
