using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine.History
{
    public class InMemoryHistory : IConsoleHistory
    {
        private readonly List<string> _history;
        private int _currentIndex;

        public InMemoryHistory()
        {
            _history = new List<string>();
            _currentIndex = -1;
        }
        
        public void Add(string entry)
        {
            _history.Insert(0, entry);
        }

        public HistoryEntry GetNextEntry()
        {
            if (GetHighestIndex() > _currentIndex)
            {
                _currentIndex++;
                return ReturnAtIndex(_currentIndex);
            }

            return null;
        }

        private int GetHighestIndex()
        {
            return _history.Count - 1;
        }

        private int GetLowestIndex()
        {
            return _history.Count > 0 ? 0 : -1;
        }

        public HistoryEntry GetPreviousEntry()
        {
            if (_currentIndex > GetLowestIndex())
            {
                _currentIndex--;
                return ReturnAtIndex(_currentIndex);
            }
            _currentIndex = -1;
            return null;
        }

        private HistoryEntry ReturnAtIndex(int index)
        {
            return new HistoryEntry(_history[index], index, false);
        }

        public void ResetHistoryMarker()
        {
            _currentIndex = -1;
        }

        public void Delete(HistoryEntry entry)
        {
            if (_history.ElementAtOrDefault(entry.Index) != null)
            {
                _history.RemoveAt(entry.Index);
            }
        }

        public void DeleteEntireHistory()
        {
            _history.Clear();
        }
    }
}
