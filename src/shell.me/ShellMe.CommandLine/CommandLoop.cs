using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ShellMe.CommandLine.CommandHandling;
using ShellMe.CommandLine.Locking;

namespace ShellMe.CommandLine
{
    public class CommandLoop
    {
        private readonly CommandFactory _commandFactory;
        private readonly ICommandPropertyWalker _commandPropertyWalker;
        private readonly ILockingService _lockingService;

        public CommandLoop() : this(new NativeConsoleWrapper())
        {}

        public CommandLoop(IConsole console) : this(console, new CommandFactory(new ICommand[]{}))
        {
        }

        public CommandLoop(IConsole console, CommandFactory commandFactory): this(console,commandFactory, new CommandPropertyWalker())
        {
        }

        public CommandLoop(IConsole console, CommandFactory commandFactory, ICommandPropertyWalker commandPropertyWalker)
        {
            Console = console;
            _commandFactory = commandFactory;
            _commandPropertyWalker = commandPropertyWalker;
            _lockingService = new FileBasedLockingService();
        }

        private IConsole Console { get; set; }

        public void Start(string[] args)
        {
            var commandMatcher = new CommandMatcher(args);
            var command = _commandFactory.GetCommand(commandMatcher.CommandName);

            if (command != null)
            {
                _commandPropertyWalker.FillCommandProperties(args, command);
            }

            var nonInteractive = command != null && command.NonInteractive;
            var exit = false;

            TryToProceedCommand(command, args);

            while (!nonInteractive && !exit)
            {
                Console.WriteLine("Enter commands or type exit to close");

                if (!nonInteractive)
                {
                    var input = Console.ReadLine();

                    if (!string.IsNullOrEmpty(input))
                    {
                        if (input.ToLower() == "exit")
                            exit = true;
                        else
                        {
                            var tempArgs = input.Split(' ');
                            command = _commandFactory.GetCommand(new CommandMatcher(tempArgs).CommandName);
                            if (TryToProceedCommand(command, tempArgs))
                                nonInteractive = command.NonInteractive;
                        }
                    }
                }
            }
        }

        private bool TryToProceedCommand(ICommand command, IEnumerable<string> args)
        {
            if (command != null)
            {
                ITraceConsole traceConsole = null;
                try
                {
                    _commandPropertyWalker.FillCommandProperties(args, command);
                    traceConsole = new TraceConsole(Console, command);
                
                    if (command.Verbose)
                        Console.WriteLine("Proceeding Command: " + command.Name);

                    if (!command.AllowParallel && !_lockingService.AcquireLock(command.Name))
                        return false;

                    command.Console = traceConsole;
                    command.Run();
                    return true;
                }
                catch (Exception exception)
                {
                    traceConsole.ForegroundColor = ConsoleColor.Red;
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("Unexpected error happended while proceeding the command: " + command.Name);
                    var exceptionWalker = new ExceptionWalker(exception);
                    foreach (var message in exceptionWalker.GetExceptionMessages())
                    {
                        stringBuilder.AppendLine(message);
                    }
                    traceConsole.TraceEvent(TraceEventType.Error, 0, stringBuilder.ToString());
                    traceConsole.ResetColor();
                    return false;
                }
                finally
                {
                    if (!command.AllowParallel)
                        _lockingService.ReleaseLock(command.Name);
                }

            }
            return false;
        }
    }
}
