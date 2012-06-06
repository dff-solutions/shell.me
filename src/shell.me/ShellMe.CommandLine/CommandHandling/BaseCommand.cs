namespace ShellMe.CommandLine.CommandHandling
{
    public abstract class BaseCommand : ICommand
    {
        protected BaseCommand()
        {
            AllowParallel = true;
        }

        public IConsole Console { get; set; }
        public bool NonInteractive { get; set; }
        public bool Verbose { get; set; }
        public bool AllowParallel { get; set; }
        public abstract string Name { get; }
        public abstract void Run();
    }
}
