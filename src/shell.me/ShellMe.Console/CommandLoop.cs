using System;
using ShellMe.Console.Configuration;

namespace ShellMe.Console
{
    public class CommandLoop
    {
        private readonly ConfigurationFactory _configurationFactory;
        private SetupProvider _setupProvider;

        public CommandLoop(IConsole console) : this(console, 
            new ConfigurationFactory(new ICommandBundle[]
            {
            }))
        {
        }

        public CommandLoop(IConsole console, ConfigurationFactory configurationFactory)
        {
            Console = console;
            _configurationFactory = configurationFactory;
        }

        public static IConsole Console { get; private set; }

        public void Start(string[] args)
        {
            _setupProvider = new SetupProvider(_configurationFactory);

            var setup = _setupProvider.GetSetup(args);
            var interactive = setup == null || setup.CommandConfiguration.Interactive;
            var exit = false;

            TryToProceedCommand(setup);

            while (interactive && !exit)
            {
                if (setup != null)
                    Console.WriteLine("commandConfiguration: " + setup.CommandConfiguration.Name);
                else
                    Console.WriteLine("Enter commands or type exit to close");

                var isValid = setup != null && setup.CommandConfiguration.IsValid;

                if (interactive && !isValid)
                {
                    var input = Console.ReadLine();

                    if (!string.IsNullOrEmpty(input))
                    {
                        if (input.ToLower() == "exit")
                            exit = true;
                        else
                            setup = _setupProvider.GetSetup(input.Split(' '));
                    }
                }

                TryToProceedCommand(setup);
                setup = null;
            }
        }

        private static void TryToProceedCommand(Setup setup)
        {
            if (setup != null)
            {
                if (setup.CommandConfiguration.IsValid)
                {
                    try
                    {
                        setup.Processor.Process();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Unexpected error happended while proceeding the command: " + setup.CommandConfiguration.Name);
                        var exceptionWalker = new ExceptionWalker(exception);
                        foreach (var message in exceptionWalker.GetExceptionMessages())
                        {
                            Console.WriteLine(message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("I recognized your command as '{0}'but there were errors:", setup.CommandConfiguration.Name));
                    setup.CommandConfiguration.ConfigurationErrors.ForEach(Console.WriteLine);
                }
            }
        }
    }
}
