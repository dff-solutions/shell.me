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
        {
            _commandQueue = commandQueue;
            OutputQueue = new List<string>();
        }

        public List<string> OutputQueue { get; private set; }

        public void WriteLine(string line)
        {
            OutputQueue.Add(line);
        }

        public void Write(char[] line)
        {
            OutputQueue.Add(line.ToString());
        }

        public void Clear()
        {
            OutputQueue.RemoveAt(OutputQueue.Count-1);
        }

        public string ReadLine()
        {
            var first = _commandQueue.First();
            _commandQueue.RemoveAt(0);
            return first;
        }

        public ConsoleKeyInfo Readkey()
        {
            var first = _commandQueue.First();
            var keyChar = string.IsNullOrEmpty(first) ? '$' : first.ToCharArray()[0];

            ConsoleKey consoleKey;
            Enum.TryParse(new string(new[] { keyChar }).ToUpper(), out consoleKey);
            var keyInfo = new ConsoleKeyInfo(keyChar, consoleKey, false, false, false);

            _commandQueue.RemoveAt(0);
            if(first.Length > 0 )
                _commandQueue.Insert(0,first.Substring(1));

            return keyInfo;
        }

        public ConsoleColor ForegroundColor { get; set; }

        public void ResetColor()
        {
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
