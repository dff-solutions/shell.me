using System;
using System.Collections.Generic;


namespace ShellMe.CommandLine
{
    public class NativeConsoleWrapper : IConsole
    {

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public void Write(char[] line)
        {
            Clear();
            Console.Write(line);
            Console.SetCursorPosition(line.Length, Console.CursorTop);
        }

        public void Clear()
        {
            int curTop = Console.CursorTop;
            int height = 1;
            int x = 0;
            int y = Console.CursorTop;
            int width = Console.BufferWidth;
            
            for (; height > 0; )
            {
                Console.SetCursorPosition(x, y + --height);
                Console.Write(new string(' ', width));
            }
            Console.SetCursorPosition(0, curTop);
        }


        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public ConsoleKeyInfo Readkey()
        {
            return Console.ReadKey();
        }


        public ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set
            {
                Console.ForegroundColor = value;
            }
        }

        public void ResetColor()
        {
            Console.ResetColor();
        }

        public int GetCursorPositionTop()
        {
            throw new NotImplementedException();
        }

        public int GetCursorPositionLeft()
        {
            throw new NotImplementedException();
        }
    }
}
