namespace ShellMe.CommandLine.CommandHandling
{
    public interface ICommand
    {
        IConsole Console { get; set; }

        bool NonInteractive { get; set; }

        bool Verbose { get; set; }

        string Name { get; }

        void Run();
    }
}
