using System;


namespace ShellMe.CommandLine
{public class NativeConsoleWrapper : IConsole
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public string ReadLine()
        {
            return Console.ReadLine();
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
