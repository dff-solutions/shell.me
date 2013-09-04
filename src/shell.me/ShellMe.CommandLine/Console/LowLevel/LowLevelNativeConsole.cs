using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ShellMe.CommandLine.Console.LowLevel
{
    public class LowLevelNativeConsole : ILowLevelConsole
    {
        public int MaxColumn
        {
            get { return System.Console.BufferWidth - 1; }
        }

        public ConsoleColor ForegroundColor
        {
            get { return System.Console.ForegroundColor; }
            set { System.Console.ForegroundColor = value; }
        }

        public int CursorLeft
        {
            get { return System.Console.CursorLeft; }
            set { System.Console.CursorLeft = value; }
        }

        public int CursorTop
        {
            get { return System.Console.CursorTop; }
            set
            {
                var top = value > Int16.MaxValue - 2 ? Int16.MaxValue - 2 : value;

                if (System.Console.BufferHeight <= top)
                    System.Console.BufferHeight = top + 1;

                System.Console.CursorTop = top;
            }
        }

        public char? ValueAtCursor
        {
            get { return ReadCharacterAt(CursorLeft, CursorTop); }
        }

        public void WriteAtCursorAndMove(char key)
        {
            System.Console.Write(key);
        }

        public ConsoleKeyInfo Read()
        {
            return System.Console.ReadKey(true);
        }

        public char? ReadCharacterAt(int x, int y)
        {
            IntPtr consoleHandle = GetStdHandle(-11);
            if (consoleHandle == IntPtr.Zero)
            {
                return null;
            }
            var position = new Coord
            {
                X = (short)x,
                Y = (short)y
            };
            var result = new StringBuilder(1);
            uint read = 0;
            if (ReadConsoleOutputCharacter(consoleHandle, result, 1, position, out read))
            {
                return result[0];
            }
            else
            {
                return null;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool ReadConsoleOutputCharacter(IntPtr hConsoleOutput, [Out] StringBuilder lpCharacter, uint length, Coord bufferCoord, out uint lpNumberOfCharactersRead);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;
        }
    }
}
