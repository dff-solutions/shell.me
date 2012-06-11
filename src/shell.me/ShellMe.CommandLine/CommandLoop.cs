using System;
using System.Collections.Generic;
using System.Globalization;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.CommandLine
{
    public class CommandLoop
    {
        private readonly CommandFactory _commandFactory;
        private readonly ICommandPropertyWalker _commandPropertyWalker;
        private readonly ICommandHistory _commandHistory;

        public CommandLoop() : this(new NativeConsoleWrapper())
        {}

        public CommandLoop(IConsole console) : this(console, new CommandFactory(new ICommand[]{}))
        {
        }

        public CommandLoop(IConsole console, CommandFactory commandFactory): this(console,commandFactory, new CommandPropertyWalker(), new InMemoryCommandHistory(console))
        {
        }

        public CommandLoop(IConsole console, CommandFactory commandFactory, ICommandHistory commandHistory)
            : this(console, commandFactory, new CommandPropertyWalker(), commandHistory)
        {
        }

        public CommandLoop(IConsole console, CommandFactory commandFactory, ICommandPropertyWalker commandPropertyWalker, ICommandHistory commandHistory)
        {
            Console = console;
            _commandFactory = commandFactory;
            _commandPropertyWalker = commandPropertyWalker;
            _commandHistory = commandHistory;

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

            var input = string.Empty;
            //Console.WriteLine("Enter commands or type exit to close");
            while (!nonInteractive && !exit)
            {

                Console.WriteLine("Enter commands or type exit to close");

                if (!nonInteractive)
                {
                    input = _commandHistory.GetCommand();
                    if (!string.IsNullOrEmpty(input) )
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
                if (command.Verbose)
                    Console.WriteLine("Proceeding Command: " + command.Name);

                try
                {
                    _commandPropertyWalker.FillCommandProperties(args, command);
                    command.Console = Console;
                    command.Run();
                    return true;
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Unexpected error happended while proceeding the command: " + command.Name);
                    var exceptionWalker = new ExceptionWalker(exception);
                    foreach (var message in exceptionWalker.GetExceptionMessages())
                    {
                        Console.WriteLine(message);
                    }
                    Console.ResetColor();
                    return false;
                }
            }
            return false;
        }
    }
}
