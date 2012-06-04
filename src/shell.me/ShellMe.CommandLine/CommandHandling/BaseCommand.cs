namespace ShellMe.CommandLine.CommandHandling
{
    public abstract class BaseCommand : ICommand
    {
        public IConsole Console { get; set; }
        public bool NonInteractive { get; set; }
        public bool Verbose { get; set; }
        public abstract string Name { get; }
        public abstract void Run();
    }
}
