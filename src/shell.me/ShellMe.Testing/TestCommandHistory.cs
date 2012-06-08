using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine
{
    public  class TestCommandHistory : ICommandHistory
    {
        private readonly List<string> _commandHistory = new List<string>();
        private int _currentCommandIndex = 0;
        private Dictionary<char, Action> _matches;
        private IConsole _console;



        public TestCommandHistory(IConsole console)
        {
            _console = console;
            _matches = new Dictionary<char, Action>
                           {
                               { '<', () => GetPrevioseCommand() },
                               { '>', () => GetNextCommand() }
                           };
        }

        public Dictionary<char, Action> Matches
        {
            get { return _matches; }
            set { _matches = value; }
        }

        public void SaveCommand(string command)
        {
            _commandHistory.Add(command);
        }

        public int GetCurrentCommandIndex()
        {
            return _currentCommandIndex;
        }

        public string GetLastCommand()
        {
            return _commandHistory[_commandHistory.Count - 1];
        }

        public string GetPrevioseCommand()
        {
            var ret = string.Empty;
            if (_currentCommandIndex < _commandHistory.Count)
            {

                ret = _commandHistory[_currentCommandIndex];
                _currentCommandIndex++;
            }
            else
            {
                ret = _commandHistory[_currentCommandIndex];
                _currentCommandIndex = _commandHistory.Count - 1;
            }
            _console.WriteLine(ret);
            return ret;
        }

        public string GetNextCommand()
        {
            if (_currentCommandIndex != 0)
                _currentCommandIndex--;
            else
                _currentCommandIndex = 0;

            return _commandHistory[_currentCommandIndex];
        }

        public string FindCommand(string searchstring)
        {
            return _commandHistory
                        .FirstOrDefault(cmd => cmd.Contains(searchstring));
        }

        public string GetCommand()
        {
            bool comandEnd = false;
            string buffer = string.Empty;
            int currentIndexOfLine = 0;

            while (!comandEnd)
            {
                var keyInfo = _console.Readkey();
                if (keyInfo.Key == ConsoleKey.Enter || keyInfo.KeyChar == '$')
                {
                    comandEnd = true;
                }
                else if (Matches.ContainsKey(keyInfo.KeyChar))
                {
                    var action = Matches[keyInfo.KeyChar];
                    action.Invoke();
                    comandEnd = true;
                    buffer = CommandStates.LastCommandWasPrintet.ToString();
                }
                else
                {
                    buffer += keyInfo.KeyChar;
                    currentIndexOfLine++;
                }
            }
            if (buffer != CommandStates.LastCommandWasPrintet.ToString() && buffer != string.Empty) 
                SaveCommand(buffer);
            return buffer;
        }
    }
}
