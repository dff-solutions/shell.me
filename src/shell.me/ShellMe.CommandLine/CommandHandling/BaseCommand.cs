using System;
using System.Collections.Generic;
using System.Diagnostics;
using ShellMe.CommandLine.Console;

namespace ShellMe.CommandLine.CommandHandling
{
    public abstract class BaseCommand : MarshalByRefObject, ICommand, ITraceableCommand
    {
        protected BaseCommand()
        {
            AllowParallel = true;
            LogLevel = new List<SourceLevels>();
            FileLogLevel = new List<SourceLevels>();
            EventLogLevel = new List<SourceLevels>();
        }

        public AbstractTraceConsole Console { get; set; }
        public bool NonInteractive { get; set; }
        public bool AllowParallel { get; set; }
        public string WriteFile { get; set; }
        public bool WriteEventLog { get; set; }
        public IEnumerable<SourceLevels> LogLevel { get; set; }
        public IEnumerable<SourceLevels> FileLogLevel { get; set; }
        public IEnumerable<SourceLevels> EventLogLevel { get; set; }
        public abstract string Name { get; }
        public abstract void Run();

        public void InjectProperties(IEnumerable<string> arguments)
        {
            var commandPropertyWalker = new CommandPropertyWalker();
            commandPropertyWalker.FillCommandProperties(arguments, this);
        }

        public override object InitializeLifetimeService()
        {
            // returning null here will prevent the lease manager
            // from deleting the object.
            return null;
        }
    }
}
