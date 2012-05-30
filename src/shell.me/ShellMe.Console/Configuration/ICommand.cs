using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellMe.Console.Configuration
{
    public interface ICommand
    {
        IConsole Console { get; set; }

        bool Interactive { get; set; }

        bool Verbose { get; set; }

        string Name { get; }

        void Run();
    }
}
