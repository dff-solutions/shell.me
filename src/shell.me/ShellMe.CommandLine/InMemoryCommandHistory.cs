using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine
{

    public class TextBuffer
    {
        public bool FromHistory { get; set; }
        public string BufferText { get; set; }
    }

    public  class InMemoryCommandHistory : ICommandHistory
    {
        private readonly List<string> _commandHistory = new List<string>();
        private int _currentCommandIndex = 0;
        private Dictionary<ConsoleKey, Action> _matches;
        private IConsole _console;
        public List<string> _up = new List<string>();
        public List<string> _down = new List<string>();
        public TextBuffer Buffer = new TextBuffer{BufferText = string.Empty,FromHistory = false};

        public InMemoryCommandHistory(IConsole console)
        {
            _console = console;
            _matches = new Dictionary<ConsoleKey, Action>
                           {
                               { ConsoleKey.UpArrow , () => GetPreviousCommand() },
                               { ConsoleKey.DownArrow , () => GetNextCommand() }
                           };
        }

        public Dictionary<ConsoleKey, Action> Matches
        {
            get { return _matches; }
            set { _matches = value; }
        }

        public void SaveCommand(string command)
        {
            _up.Add(command);
        }

        public int GetCurrentCommandIndex()
        {
            return _currentCommandIndex;
        }

        public string GetLastCommand()
        {
            return _commandHistory[_commandHistory.Count - 1];
        }

        public string GetPreviousCommand()
        {
            if (_up.Count == 0) return "";

            var cmd = _up.Last();
            _down.Add(cmd);
            _console.Clear();
            _console.Write(cmd.ToCharArray());
            Buffer.BufferText = cmd;
            Buffer.FromHistory = true;
            _up.RemoveAt(_up.Count-1);
            return cmd;
        }

        public string GetNextCommand()
        {
            if (_down.Count == 0) return "";
            
            var cmd = _down.Last();
            _up.Add(cmd);
            _console.Clear();
            _console.Write(cmd.ToCharArray());
            Buffer.BufferText = cmd;
            Buffer.FromHistory = true;
            _down.RemoveAt(_down.Count - 1);
            return cmd;
        }

        public string GetCommand()
        {
            bool comandEnd = false;

            int currentIndexOfLine = 0;
            var buffer = string.Empty;
            Buffer = new TextBuffer() {BufferText = string.Empty, FromHistory = false};
            while (!comandEnd)
            {
                var keyInfo = _console.Readkey();
                if (keyInfo.Key == ConsoleKey.Enter || keyInfo.KeyChar == '$')
                {
                    buffer = Buffer.BufferText;
                    comandEnd = true;
                }
                else if (Matches.ContainsKey(keyInfo.Key))
                {
                    Action action = Matches[keyInfo.Key];
                    action.Invoke();
                }
                else
                {
                    Buffer.BufferText += keyInfo.KeyChar;
                    Buffer.FromHistory = false;
                    currentIndexOfLine++;
                }
            }
            if (!Buffer.FromHistory && buffer != string.Empty)
                SaveCommand(buffer);

            // down reversen und append to up
            _down.Reverse();
            _up.AddRange(_down);
            _down.Clear();
            if (Buffer.FromHistory)
                return Buffer.BufferText.Replace("\b", "").Replace("\0", "");
            else
                return buffer.Replace("\b", "").Replace("\0", "");
        }

    }
}
