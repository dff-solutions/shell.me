using System;
using System.Collections.Generic;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.Testing
{
    public class MaliciousCommand : BaseCommand
    {
        public override string Name
        {
            get { return "Malicious"; }
        }

        public override void Run()
        {
            Console.WriteLine("Should not see me");
        }

        public int Value
        {
            get { return 0; }
            set {throw new Exception("Should not happen");}
        }
    }
}
