using ShellMe.Console.Configuration;

namespace ShellMe.Console.CommandHandling
{
    public abstract class BaseCommand : ICommand
    {
        public IConsole Console { get; set; }
        public bool Interactive { get; set; }
        public bool Verbose { get; set; }
        public abstract string Name { get; }
        public abstract void Run();
    }
}
