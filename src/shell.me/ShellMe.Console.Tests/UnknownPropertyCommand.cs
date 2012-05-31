using System.Drawing;
using ShellMe.Console.CommandHandling;

namespace ShellMe.Console.Tests
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
