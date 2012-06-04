using System;

namespace ShellMe.CommandLine
{
    public interface IConsole
    {
        void WriteLine(string line);
        string ReadLine();
        ConsoleColor ForegroundColor { get; set; }
        void ResetColor();
    }
}
