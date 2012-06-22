namespace ShellMe.CommandLine.CommandHandling
{
    public interface ICommand
    {
        AbstractTraceConsole Console { get; set; }

        bool NonInteractive { get; set; }

        bool Verbose { get; set; }

        bool AllowParallel { get; set; }

        string Name { get; }

        void Run();
    }
}
