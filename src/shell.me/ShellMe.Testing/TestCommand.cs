using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.Testing
{
    public class TestCommand : BaseCommand
    {
        public override string Name
        {
            get { return "Test"; }
        }

        public override void Run()
        {
            Console.WriteLine(string.Format("Run. Test: {0}, Text: {1}", IsTest, Text));
        }

        public bool IsTest { get; set; }

        public string Text { get;set; }
    }
}
