using System.Collections.Generic;
using System.Diagnostics;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.Testing
{
    public class LogLevelCommand : BaseCommand
    {
        public override string Name
        {
            get { return "LogLevel"; }
        }

        public override void Run()
        {
            foreach (var logLevel in LogLevel)
            {
                Console.WriteLine(logLevel.ToString());
            }

            Console.TraceEvent(TraceEventType.Error, 1, "error");
            Console.TraceEvent(TraceEventType.Verbose, 1, "verbose");
            Console.TraceEvent(TraceEventType.Information, 1, "information");
        }
    }
}
