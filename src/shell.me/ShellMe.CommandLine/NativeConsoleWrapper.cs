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
            Console.Write(line);
        }

        public void Clear()
        {
            Console.Clear();
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

    }
}
