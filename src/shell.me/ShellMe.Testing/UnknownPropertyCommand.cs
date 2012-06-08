using System.Drawing;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.Testing
{
    public class UnknownPropertyCommand : BaseCommand
    {
        public override string Name
        {
            get { return "UnknownProperty"; }
        }

        public override void Run()
        {
            Console.WriteLine(Size.ToString());
        }

        public Point Size { get; set; }
    }
}
