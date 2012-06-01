using System.Drawing;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.CommandLine.Tests
{
    class UnknownPropertyCommand : BaseCommand
    {
        public override string Name
        {
            get { return "Test"; }
        }

        public override void Run()
        {
            Console.WriteLine(Size.ToString());
        }

        public Point Size { get; set; }
    }
}
