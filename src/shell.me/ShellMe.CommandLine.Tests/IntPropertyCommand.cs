using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.CommandLine.Tests
{
    class IntPropertyCommand : BaseCommand
    {
        public override string Name
        {
            get { return "Test"; }
        }

        public override void Run()
        {
            Console.WriteLine(Size.ToString());
        }

        public int Size { get; set; }
    }
}
