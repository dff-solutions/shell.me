using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine
{
    public class InMemoryCommandHistory : ICommandHistory
    {
        private readonly List<string> _commandHistory = new List<string>();
        private int _currentCommandIndex = 0;

        private Dictionary<ConsoleKey, Action> _matches;

        public InMemoryCommandHistory()
        {
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
            _currentCommandIndex++;
            return _commandHistory[_currentCommandIndex];
        }

        public string GetNextCommand()
        {
            _currentCommandIndex--;
            return _commandHistory[_currentCommandIndex];
        }

        public string FindCommand(string searchstring)
        {
            return _commandHistory
                        .FirstOrDefault(cmd => cmd.Contains(searchstring));
        }
    }
}
