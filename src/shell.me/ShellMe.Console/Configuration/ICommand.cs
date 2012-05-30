using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellMe.Console.Configuration
{
    public interface ICommand
    {
        string Name { get; }

        void Run();
    }
}
