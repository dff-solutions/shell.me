namespace ShellMe.CommandLine.CommandHandling
{
    public interface ICommand
    {
        IConsole Console { get; set; }

        bool Interactive { get; set; }

        bool Verbose { get; set; }

        string Name { get; }

        void Run();
    }
}
