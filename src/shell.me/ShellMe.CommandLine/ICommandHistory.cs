﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine
{
    public interface ICommandHistory
    {
        void   SaveCommand(string command);
        int GetCurrentCommandIndex();
        string GetLastCommand();
        string GetPreviousCommand();
        string GetNextCommand();
        string GetCommand();
    }
}