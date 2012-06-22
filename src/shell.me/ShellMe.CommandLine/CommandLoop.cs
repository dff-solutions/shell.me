using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public CommandLoop(AbstractConsole console) : this(console, new CommandFactory())
        {
        }

        public CommandLoop(AbstractConsole console, CommandFactory commandFactory): this(console,commandFactory, new CommandPropertyWalker())
        {
        }

        public CommandLoop(AbstractConsole console, CommandFactory commandFactory, ICommandPropertyWalker commandPropertyWalker)
        {
            Console = console;
            _commandFactory = commandFactory;
            _commandPropertyWalker = commandPropertyWalker;
            _lockingService = new FileBasedLockingService();
        }

        private AbstractConsole Console { get; set; }

        public void Start(string[] args)
        {
            Func<string, string[]> splitCommand = input => input.Split(new[]{" -"}, StringSplitOptions.None);

            //The standard argument parsing does not handle inputs like --logLevel=[Error, Information] as we want them
            //to be treated. That's why we first add a whitespace to each fragment, then combine them again, and
            //then split them again the way we want it.
            args = splitCommand(string.Concat(args.Select(fragment => " " + fragment)));

            var commandMatcher = new CommandMatcher(args);
            var command = _commandFactory.GetCommand(commandMatcher.CommandName);

            if (command != null)
            {
                try
                {
                    _commandPropertyWalker.FillCommandProperties(args, command);
                    TryToProceedCommand(command, args);
                }
                catch (Exception exception)
                {
                    LogException(new TraceConsole(Console,command), exception, command);
                }
            }

            var nonInteractive = command != null && command.NonInteractive;
            var exit = false;

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
                            var tempArgs = splitCommand(input);
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
                AbstractTraceConsole traceConsole = null;
                try
                {
                    _commandPropertyWalker.FillCommandProperties(args, command);
                    traceConsole = new TraceConsole(Console, command);
                
                    if (command.Verbose)
                        traceConsole.TraceEvent(TraceEventType.Verbose, 0, "Proceeding Command: " + command.Name);

                    if (!command.AllowParallel && !_lockingService.AcquireLock(command.Name))
                        return false;

                    command.Console = traceConsole;
                    command.Run();
                    return true;
                }
                catch (Exception exception)
                {
                    LogException(traceConsole, exception, command);
                    return false;
                }
                finally
                {
                    if (!command.AllowParallel)
                        _lockingService.ReleaseLock(command.Name);

                    var disposable = traceConsole as IDisposable;
                    if (disposable != null)
                        disposable.Dispose();
                }

            }
            return false;
        }

        private void LogException(AbstractConsole console, Exception exception, ICommand command)
        {
            if (console == null)
                console = Console;

            var traceconsole = console as TraceConsole;
            console.ForegroundColor = ConsoleColor.Red;
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Unexpected error happended while proceeding the command: " + command.Name);
            var exceptionWalker = new ExceptionWalker(exception);
            foreach (var message in exceptionWalker.GetExceptionMessages())
            {
                stringBuilder.AppendLine(message);
            }

            if(traceconsole != null)
            {
                //If we have a TraceConsole, trace as error, otherwise use the default Console
                try
                {
                    traceconsole.TraceEvent(TraceEventType.Error, 0, stringBuilder.ToString());
                }
                catch (Exception ex)
                {
                    //In case of any errors regardings the tracing (e.g. missing FileWrite rights) use 
                    //the default console and log this exception as well
                    Console.WriteLine(stringBuilder.ToString());
                    LogException(Console, ex, command);
                }
                
            }
            else
            {
                Console.WriteLine(stringBuilder.ToString());
            }

            console.ResetColor();
        }
    }
}
