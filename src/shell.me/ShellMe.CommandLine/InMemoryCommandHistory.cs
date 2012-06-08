﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine
{
    public enum CommandStates
    {
        None = 0,
        LastCommandWasPrintet = 1
    }

    public  class InMemoryCommandHistory : ICommandHistory
    {
        private readonly List<string> _commandHistory = new List<string>();
        private int _currentCommandIndex = 0;
        private Dictionary<ConsoleKey, Action> _matches;
        private IConsole _console;

        public InMemoryCommandHistory(IConsole console)
        {
            _console = console;
            _matches = new Dictionary<ConsoleKey, Action>
                           {
                               { ConsoleKey.UpArrow , () => GetPrevioseCommand() },
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
            if(_currentCommandIndex < _commandHistory.Count)
                _currentCommandIndex++;
            else
            {
                _currentCommandIndex = _commandHistory.Count - 1;
            }
            return _commandHistory[_currentCommandIndex];
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
                else if (Matches.ContainsKey(keyInfo.Key))
                {
                    Action action = Matches[keyInfo.Key];
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
