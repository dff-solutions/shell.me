using System;

namespace ShellMe.CommandLine
{
    public interface IConsole
    {
        void WriteLine(string line);
        void Write(char[] line);
        void Clear();
        string ReadLine();
        ConsoleKeyInfo Readkey();
        ConsoleColor ForegroundColor { get; set; }
        void ResetColor();
        int GetCursorPositionTop();
        int GetCursorPositionLeft();
    }
}
