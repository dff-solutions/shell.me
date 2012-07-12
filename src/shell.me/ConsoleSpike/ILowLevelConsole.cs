using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleSpike
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
