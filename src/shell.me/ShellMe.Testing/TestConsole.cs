using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShellMe.CommandLine;

namespace ShellMe.Testing
{
    public class TestConsole : IConsole
    {
        private readonly List<string> _commandQueue;

        public TestConsole(List<string> commandQueue)
            : this(new InMemoryCommandHistory(),commandQueue)
        {}

        public TestConsole(InMemoryCommandHistory commandHistory,List<string> commandQueue)
        {
            _commandQueue = commandQueue;
            OutputQueue = new List<string>();
            CommandHistory = commandHistory;
        }

        public InMemoryCommandHistory CommandHistory { get; set; }

        public List<string> OutputQueue { get; private set; }

        public void WriteLine(string line)
        {
            OutputQueue.Add(line);
        }

        public string ReadLine()
        {
            var first = _commandQueue.First();
            string buffer = string.Empty;
            foreach (var character in first)
            {
                ConsoleKey consoleKey;
                Enum.TryParse(new string(new[] {character}).ToUpper(), out consoleKey);

                var keyInfo = new ConsoleKeyInfo(character, consoleKey, false, false, false);

                if (!CommandHistory.Matches.ContainsKey(keyInfo.Key))
                {
                    buffer += keyInfo.KeyChar;
                }
                else
                {
                    Action action = CommandHistory.Matches[keyInfo.Key];
                    action.Invoke();
                }
            }

            _commandQueue.RemoveAt(0);
            return buffer;
        }

        public ConsoleColor ForegroundColor { get; set; }

        public void ResetColor()
        {
        }
    }
}
