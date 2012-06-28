using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.Testing
{
    public class IntPropertyCommand : BaseCommand
    {
        public override string Name
        {
            get { return "IntProperty"; }
        }

        public override void Run()
        {
            Console.WriteLine(Size.ToString());
        }

        public int Size { get; set; }
    }
}
