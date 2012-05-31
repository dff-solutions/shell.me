using ShellMe.Console.CommandHandling;
using ShellMe.Console.Configuration;

namespace ShellMe.Console.Tests
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
