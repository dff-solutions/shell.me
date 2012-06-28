using System;


namespace ShellMe.CommandLine
{public class NativeConsoleWrapper : AbstractConsole
    {
        public override void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public override string ReadLine()
        {
            return Console.ReadLine();
        }

        public override ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set
            {
                Console.ForegroundColor = value;
            }
        }

        public override void ResetColor()
        {
            Console.ResetColor();
        }
    }
}
