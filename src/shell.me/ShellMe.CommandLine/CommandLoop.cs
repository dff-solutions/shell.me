using System;
using System.Collections.Generic;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.CommandLine
{
    public class CommandLoop
    {
        private readonly CommandFactory _commandFactory;
        private readonly ICommandPropertyWalker _commandPropertyWalker;

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
        }

        public static IConsole Console { get; private set; }

        public void Start(string[] args)
        {
            var commandMatcher = new CommandMatcher(args);
            var command = _commandFactory.GetCommand(commandMatcher.CommandName);

            if (command != null)
            {
                _commandPropertyWalker.FillCommandProperties(args, command);
            }

            var interactive = command == null || command.Interactive;
            var exit = false;

            TryToProceedCommand(command, args);

            while (interactive && !exit)
            {
                Console.WriteLine("Enter commands or type exit to close");

                if (interactive)
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
                            TryToProceedCommand(command, tempArgs);
                            interactive = command.Interactive;
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
                    Console.WriteLine("Unexpected error happended while proceeding the command: " + command.Name);
                    var exceptionWalker = new ExceptionWalker(exception);
                    foreach (var message in exceptionWalker.GetExceptionMessages())
                    {
                        Console.WriteLine(message);
                    }
                    return false;
                }
            }
            return false;
        }
    }
}
