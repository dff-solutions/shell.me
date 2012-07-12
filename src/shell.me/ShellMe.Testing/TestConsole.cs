using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShellMe.CommandLine;
using ShellMe.CommandLine.Console;

namespace ShellMe.Testing
{
    public class TestConsole : AbstractConsole
    {
        private readonly List<string> _commandQueue;

        public TestConsole(List<string> commandQueue)
        {
            _commandQueue = commandQueue;
            OutputQueue = new List<string>();
        }

        public List<string> OutputQueue { get; private set; }

        public override void WriteLine(string line)
        {
            OutputQueue.Add(line);
        }

        public override string ReadLine()
        {
            var first = _commandQueue.First();
            _commandQueue.RemoveAt(0);
            return first;
        }

        public override ConsoleColor ForegroundColor { get; set; }
    }
}
