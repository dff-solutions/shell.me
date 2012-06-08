using System;

namespace ShellMe.CommandLine
{
    public interface IConsole
    {
        void WriteLine(string line);
        string ReadLine();
        ConsoleKeyInfo Readkey();
        ConsoleColor ForegroundColor { get; set; }
        void ResetColor();
    }
}
