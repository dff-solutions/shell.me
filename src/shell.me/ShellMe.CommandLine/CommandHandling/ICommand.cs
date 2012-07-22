using System.Collections.Generic;
using ShellMe.CommandLine.Console;

namespace ShellMe.CommandLine.CommandHandling
{
    public interface ICommand
    {
        AbstractTraceConsole Console { get; set; }

        bool NonInteractive { get; set; }

        bool AllowParallel { get; set; }

        string Name { get; }

        void Run();

        void InjectProperties(IEnumerable<string> arguments);
    }
}
