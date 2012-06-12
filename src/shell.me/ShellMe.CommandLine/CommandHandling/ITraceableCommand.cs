using System.Collections.Generic;
using System.Diagnostics;

namespace ShellMe.CommandLine.CommandHandling
{
    public interface ITraceableCommand
    {
        string WriteFile { get; set; }
        bool WriteEventLog { get; set; }
        IEnumerable<SourceLevels> LogLevel { get; set; }
        IEnumerable<SourceLevels> FileLogLevel { get; set; }
        IEnumerable<SourceLevels> EventLogLevel { get; set; }
    }
}
