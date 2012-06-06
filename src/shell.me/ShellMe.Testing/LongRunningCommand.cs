using System.Threading;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.Testing
{
    public class LongRunningCommand : BaseCommand
    {
        public override string Name
        {
            get { return "LongRunningCommand"; }
        }

        public override void Run()
        {
            Thread.Sleep(500);
            Console.WriteLine("Completed");
        }

        public bool IsTest { get; set; }

        public string Text { get;set; }
    }
}
