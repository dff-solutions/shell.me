using System;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.CommandLine.Tests
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
