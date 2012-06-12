using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShellMe.CommandLine.CommandHandling;

namespace TestCommands
{
    class TestCommands1 : BaseCommand
    {
        public override string Name
        {
            get { return "Test1"; }
        }

        public bool Log { get; set; }

        public override void Run()
        {
            Console.WriteLine(string.Format("Run Test1 arg = {0}",Log));
        }
    }

    class TestCommands2 : BaseCommand
    {
        public override string Name
        {
            get { return "Test2"; }
        }

        public override void Run()
        {
            Console.WriteLine("Run Test2");
        }
    }

    class TestCommands3 : BaseCommand
    {
        public override string Name
        {
            get { return "Test3"; }
        }

        public override void Run()
        {
            Console.WriteLine("Run Test3");
        }
    }
}
