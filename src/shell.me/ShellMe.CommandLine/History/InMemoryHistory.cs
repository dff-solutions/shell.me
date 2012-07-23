using System.Collections.Generic;
using System.Linq;

namespace ShellMe.CommandLine.History
{
    public class InMemoryHistory : IConsoleHistory
    {
        protected List<string> History { get; private set; }
        private int _currentIndex;

        public InMemoryHistory() :this(new List<string>())
        {
        }
        
        protected InMemoryHistory(List<string> history)
        {
            History = history;
            _currentIndex = -1;
        }

        public virtual void Add(string entry)
        {
            History.Insert(0, entry);
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
            return History.Count - 1;
        }

        private int GetLowestIndex()
        {
            return History.Count > 0 ? 0 : -1;
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
            return new HistoryEntry(History[index], index, false);
        }

        public void ResetHistoryMarker()
        {
            _currentIndex = -1;
        }

        public void Delete(HistoryEntry entry)
        {
            if (History.ElementAtOrDefault(entry.Index) != null)
            {
                History.RemoveAt(entry.Index);
            }
        }

        public void DeleteEntireHistory()
        {
            History.Clear();
        }
    }
}
