using ShellMe.Console.Configuration;

namespace ShellMe.Console.Tests
{
    class TestCommand : ICommand
    {
        public string Name
        {
            get { return "Test"; }
        }

        public void Run()
        {
            throw new System.NotImplementedException();
        }

        public bool IsTest { get; set; }
    }
}
