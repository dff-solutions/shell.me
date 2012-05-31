using System;
using ShellMe.Console.CommandHandling;
using ShellMe.Console.Configuration;

namespace ShellMe.Console.Tests
{
    class ExceptionCommand : BaseCommand
    {
        public override string Name
        {
            get { return "RaiseException"; }
        }

        public override void Run()
        {
            throw new NotImplementedException("Foo", new Exception("Bar"));
        }
    }
}
