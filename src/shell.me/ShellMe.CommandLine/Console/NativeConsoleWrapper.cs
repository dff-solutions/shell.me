using System;

namespace ShellMe.CommandLine.Console
{
    public class NativeConsoleWrapper : AbstractConsole
    {
        public override void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }

        public override string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public override string CurrentInput
        {
            get { throw new NotImplementedException(); }
        }

        public override ConsoleColor ForegroundColor
        {
            get { return System.Console.ForegroundColor; }
            set
            {
                System.Console.ForegroundColor = value;
            }
        }
    }
}
