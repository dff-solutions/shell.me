using System.Collections.Generic;
using System.Linq;
using ShellMe.CommandLine.Extensions;

namespace ShellMe.CommandLine.History
{
    public class InMemoryHistory : IConsoleHistory
    {
        private int _currentIndex;

        public InMemoryHistory() :this(new List<string>())
        {
        }

        protected InMemoryHistory(List<string> history)
        {
            MaxElements = 100;
            History = history;
            _currentIndex = -1;
        }

        public int MaxElements { get; set; }

        protected List<string> History { get; private set; }

        public virtual void Add(string entry)
        {
            History.Insert(0, entry);
            RestrictToMaxSize();
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
            return History.Any() ? 0 : -1;
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

        protected void RestrictToMaxSize()
        {
            if(History.Count > MaxElements)
            {
                History
                    .ToList()
                    .Skip(MaxElements)
                    .ForEach(entry => History.Remove(entry));
            }
        }
    }
}
