﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine.History
{
    public interface IConsoleHistory
    {
        void Add(string entry);

        HistoryEntry GetNextEntry();

        HistoryEntry GetPreviousEntry();

        void ResetHistoryMarker();

        void Delete(HistoryEntry entry);

        void DeleteEntireHistory();
        int MaxElements { get; set; }
    }

    public class HistoryEntry
    {
        public HistoryEntry(string value, int index, bool isCommand)
        {
            Value = value;
            Index = index;
            IsCommand = isCommand;
        }

        public string Value { get; private set; }

        public int Index { get; private set; }

        public bool IsCommand { get; private set; }
    }
}
