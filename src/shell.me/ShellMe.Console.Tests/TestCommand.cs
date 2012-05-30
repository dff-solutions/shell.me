using ShellMe.Console.Configuration;

namespace ShellMe.Console.Tests
{
    class TestCommand : BaseCommand
    {
        public override string Name
        {
            get { return "Test"; }
        }

        public override void Run()
        {
            string text = IsTest ? "Run with Test" : "Run";
            Console.WriteLine(text);
        }

        public bool IsTest { get; set; }
    }
}
