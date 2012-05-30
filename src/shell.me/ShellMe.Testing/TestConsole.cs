using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShellMe.Console;

namespace ShellMe.Testing
{
    public class TestConsole : IConsole
    {
        private readonly List<string> _commandQueue;

        public TestConsole(List<string> commandQueue)
        {
            _commandQueue = commandQueue;
            OutputQueue = new List<string>();
        }

        public List<string> OutputQueue { get; private set; }

        public void WriteLine(string line)
        {
            OutputQueue.Add(line);
        }

        public string ReadLine()
        {
            var first = _commandQueue.First();
            _commandQueue.RemoveAt(0);
            return first;
        }
    }
}
