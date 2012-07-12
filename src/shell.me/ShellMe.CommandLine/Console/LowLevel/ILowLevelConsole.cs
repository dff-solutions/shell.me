using System;

namespace ShellMe.CommandLine.Console.LowLevel
{
    public interface ILowLevelConsole
    {
        int MaxColumn { get; }

        ConsoleColor ForegroundColor { get; set; }

        int CursorLeft { get; set; }

        int CursorTop { get; set; }

        char? ValueAtCursor { get; }

        void WriteAtCursorAndMove(char key);

        ConsoleKeyInfo Read();

        char? ReadCharacterAt(int x, int y);
    }
}
