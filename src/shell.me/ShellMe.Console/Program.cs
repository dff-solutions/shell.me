using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShellMe.CommandLine;

namespace ShellMe.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandLoop = new CommandLoop();
            commandLoop.Start(args);
        }
    }
}
