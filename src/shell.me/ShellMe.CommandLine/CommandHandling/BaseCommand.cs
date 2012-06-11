namespace ShellMe.CommandLine.CommandHandling
{
    public abstract class BaseCommand : ICommand, ITraceableCommand
    {
        protected BaseCommand()
        {
            AllowParallel = true;
        }

        public ITraceConsole Console { get; set; }
        public bool NonInteractive { get; set; }
        public bool AllowParallel { get; set; }
        public bool Verbose { get; set; }
        public string WriteFile { get; set; }
        public bool WriteEventLog { get; set; }

        public abstract string Name { get; }
        public abstract void Run();
    }
}
