using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ShellMe.CommandLine.CommandHandling;
using ShellMe.CommandLine.Console;
using ShellMe.CommandLine.Console.LowLevel;
using ShellMe.CommandLine.History;
using ShellMe.CommandLine.Locking;
using ShellMe.CommandLine.Extensions;
using System.Reactive.Linq;

namespace ShellMe.CommandLine
{
    public class CommandLoop
    {
        private readonly ICommandFactory _commandFactory;
        private readonly ILockingService _lockingService;
        private readonly IConsoleHistory _history;

        public CommandLoop(): this(new Configuration())
        {}

        //Todo: Throw these four constructors away once we finished porting the rest of the code over to the configuration object
        public CommandLoop(ILowLevelConsole console) : this(console, new CommandFactory())
        {
        }

        public CommandLoop(ILowLevelConsole console, ICommandFactory commandFactory):this(console, commandFactory, new FileBasedLockingService())
        {
        }

        public CommandLoop(ILowLevelConsole console, ICommandFactory commandFactory, ILockingService lockingService):this(console, commandFactory, lockingService, new FileBasedHistory())
        {
        }

        public CommandLoop(ILowLevelConsole console, ICommandFactory commandFactory, ILockingService lockingService, IConsoleHistory consoleHistory):
            this(
            new Configuration()
            .UseConsole(console)
            .UseCommandFactory(commandFactory)
            .UseLockingService(lockingService)
            .UseConsoleHistory(consoleHistory)
            )
        {
        }

        public CommandLoop(Configuration configuration)
        {
            var adapter = new LowLevelToAbstractConsoleAdapter(configuration.Console);
            Console = adapter;
            _commandFactory = configuration.CommandFactory;
            _lockingService = configuration.LockingService;
            _history = configuration.ConsoleHistory;
            InitializeHistory(adapter);
        }

        private void InitializeHistory(LowLevelToAbstractConsoleAdapter adapter)
        {
            var keyMap = new Dictionary<ConsoleKey, Func<HistoryEntry>>
                             {
                                 { ConsoleKey.UpArrow, () => _history.GetNextEntry() },
                                 { ConsoleKey.DownArrow, () => _history.GetPreviousEntry() }
                             };

            adapter.KeyStrokes
                .Select(keyInfo => keyInfo.Key)
                .Where(key => key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow)
                .Select(key => keyMap[key])
                .Subscribe(func =>
                               {
                                   adapter.EraseCurrentLine();
                                   var historyEntry = func();

                                   if (historyEntry != null)
                                   {
                                       adapter.Write(historyEntry.Value);
                                   }
                               });
        }

        protected AbstractConsole Console { get; set; }

        public void Start(string[] args)
        {
            Func<string, string[]> splitCommand = input => input.Split(new[]{" -"}, StringSplitOptions.None);

            //The standard argument parsing does not handle inputs like --logLevel=[Error, Information] as we want them
            //to be treated. That's why we first add a whitespace to each fragment, then combine them again, and
            //then split them again the way we want it.
            args = splitCommand(string.Concat(args.Select(fragment => " " + fragment)));

            WriteGreeting();

            if (args.Contains("-debug"))
            {
                Console.WriteLine("Attach a debugger and hit any key to continue");
                Console.ReadLine();
            }

            var commandMatcher = new CommandMatcher(args);
            var command = _commandFactory.GetCommand(commandMatcher.CommandName);

            if (command != null)
            {
                try
                {
                    TryToProceedCommand(command, args);
                }
                catch (Exception exception)
                {
                    LogException(new TraceConsole(Console,command), exception, command);
                }
            }
            else
                NotifyOnUnknownCommand(commandMatcher.CommandName);

            var nonInteractive = command != null && command.NonInteractive;
            var exit = false;

            while (!nonInteractive && !exit)
            {
                if (!nonInteractive)
                {
                    var input = Console.ReadLine().Trim();

                    if (!string.IsNullOrEmpty(input))
                    {
                        _history.Add(input);

                        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                            exit = true;
                        else if(input.Equals("clear history", StringComparison.OrdinalIgnoreCase))
                        {
                            _history.DeleteEntireHistory();
                            ConsoleHelper.WriteLineInGreen(Console,"Deleted all history entries");
                        }
                        else if(input.Equals("list commands", StringComparison.OrdinalIgnoreCase))
                        {
                            _commandFactory.GetAvailable().ForEach(c => ConsoleHelper.WriteLineInGreen(Console, c.Name));
                        }
                        else
                        {
                            var tempArgs = splitCommand(input);
                            var commandName = new CommandMatcher(tempArgs).CommandName;
                            command = _commandFactory.GetCommand(commandName);

                            if (command == null)
                                NotifyOnUnknownCommand(commandName);

                            if (TryToProceedCommand(command, tempArgs))
                                nonInteractive = command.NonInteractive;
                        }
                    }

                    _history.ResetHistoryMarker();
                }
            }
        }

        protected virtual void WriteGreeting()
        {
            var colorResetPoint = ConsoleHelper.CreateColorResetPoint(Console);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(string.Format("Welcome to shell.me v.{0}", typeof(CommandLoop).Assembly.GetName().Version));
            Console.WriteLine(" ");
            Console.WriteLine("Fork us on github!");
            Console.WriteLine("We are MIT licensed: github.com/dff-solutions/shell.me");
            Console.WriteLine(" ");
            Console.WriteLine("Enter <commandname>, use <list commands> or type <exit> to close");

            Console.WriteLine("");
            colorResetPoint();
        }

        protected virtual void NotifyOnUnknownCommand(string commandName)
        {
            if (!string.IsNullOrEmpty(commandName))
            {
                ConsoleHelper.WriteLineInRed(Console, string.Format("Unknown command: {0}", commandName));
            }
        }

        private bool TryToProceedCommand(ICommand command, IEnumerable<string> args)
        {
            if (command != null)
            {
                AbstractTraceConsole traceConsole = null;
                try
                {
                    command.InjectProperties(args);
                    traceConsole = new TraceConsole(Console, command);

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
                    ConsoleHelper.WriteLineInRed(Console, stringBuilder.ToString());
                    LogException(Console, ex, command);
                }
                
            }
            else
            {
                ConsoleHelper.WriteLineInRed(Console, stringBuilder.ToString());
            }
        }
    }
}
