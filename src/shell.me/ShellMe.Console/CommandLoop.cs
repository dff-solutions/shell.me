using System;
using ShellMe.Console.Configuration;

namespace ShellMe.Console
{
    public class CommandLoop
    {
        private readonly CommandFactory _commandFactory;
        //private SetupProvider _setupProvider;

        public CommandLoop(IConsole console) : this(console, 
            new CommandFactory(new ICommand[]
            {
            }))
        {
        }

        public CommandLoop(IConsole console, CommandFactory commandFactory)
        {
            Console = console;
            _commandFactory = commandFactory;
        }

        public static IConsole Console { get; private set; }

        public void Start(string[] args)
        {
            //_setupProvider = new SetupProvider(_commandFactory);
            var argumentsProvider = new ArgumentsProvider(args);
            var command = _commandFactory.GetCommand(argumentsProvider);
            var interactive = true; //command == null || command.CommandConfiguration.Interactive;
            var exit = false;

            TryToProceedCommand(command);

            //while (interactive && !exit)
            //{
            //    if (command != null)
            //        Console.WriteLine("commandConfiguration: " + command.CommandConfiguration.Name);
            //    else
            //        Console.WriteLine("Enter commands or type exit to close");

            //    var isValid = command != null && command.CommandConfiguration.IsValid;

            //    if (interactive && !isValid)
            //    {
            //        var input = Console.ReadLine();

            //        if (!string.IsNullOrEmpty(input))
            //        {
            //            if (input.ToLower() == "exit")
            //                exit = true;
            //            else
            //                command = _setupProvider.GetSetup(input.Split(' '));
            //        }
            //    }

            //    TryToProceedCommand(command);
            //    command = null;
            //}
        }

        private static void TryToProceedCommand(ICommand command)
        {
            if (command != null)
            {
                try
                {
                    command.Run();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Unexpected error happended while proceeding the command: " + command.Name);
                    var exceptionWalker = new ExceptionWalker(exception);
                    foreach (var message in exceptionWalker.GetExceptionMessages())
                    {
                        Console.WriteLine(message);
                    }
                }
            }
        }
    }
}
