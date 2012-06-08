using System;
using System.Collections.Generic;


namespace ShellMe.CommandLine
{public class NativeConsoleWrapper : IConsole
    {
    

        public NativeConsoleWrapper()
            : this(new InMemoryCommandHistory())
        {}

        public NativeConsoleWrapper(InMemoryCommandHistory commandHistory)
        {
            CommandHistory = commandHistory;
        }
        
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }


        public InMemoryCommandHistory CommandHistory { get; set; }

        public string ReadLine()
            {
                string buffer = string.Empty;
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        var keyInfo = Console.ReadKey();
                        if (!CommandHistory.Matches.ContainsKey(keyInfo.Key))
                        {
                            buffer += keyInfo.KeyChar;
                        }
                        else
                        {
                            
                        }
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Enter);

                return buffer; // Console.ReadLine();
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
