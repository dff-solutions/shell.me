using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShellMe.CommandLine.CommandHandling;
using ShellMe.CommandLine.Console.LowLevel;
using ShellMe.CommandLine.History;
using ShellMe.CommandLine.Locking;

namespace ShellMe.CommandLine
{
    public class Configuration
    {
        private ILowLevelConsole _console;
        public ILowLevelConsole Console
        {
            get { return _console ?? new LowLevelNativeConsole(); }
            private set { _console = value; }
        }

        private ICommandFactory _commandFactory;
        public ICommandFactory CommandFactory
        {
            get { return _commandFactory ?? new CommandFactory(); }
            private set { _commandFactory = value; }
        }

        private ILockingService _lockingService;
        public ILockingService LockingService
        {
            get { return _lockingService ?? new FileBasedLockingService(); }
            private set { _lockingService = value; }
        }

        private IConsoleHistory _consoleHistory;
        public IConsoleHistory ConsoleHistory
        {
            get { return _consoleHistory ?? new FileBasedHistory(); }
            set { _consoleHistory = value; }
        }

        public Configuration UseConsole(ILowLevelConsole console)
        {
            RaiseExceptionOnNull(console, "console");
            Console = console;
            return this;
        }

        public Configuration UseCommandFactory(ICommandFactory commandFactory)
        {
            RaiseExceptionOnNull(commandFactory, "commandFactory");
            CommandFactory = commandFactory;
            return this;
        }

        public Configuration UseLockingService(ILockingService lockingService)
        {
            RaiseExceptionOnNull(lockingService, "lockingService");
            LockingService = lockingService;
            return this;
        }

        public Configuration UseConsoleHistory(IConsoleHistory consoleHistory)
        {
            RaiseExceptionOnNull(consoleHistory, "consoleHistory");
            ConsoleHistory = consoleHistory;
            return this;
        }

        private void RaiseExceptionOnNull(object value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }
    }
}
