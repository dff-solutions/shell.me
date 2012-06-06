using System;

namespace ShellMe.CommandLine.Locking
{
    internal interface ILockingService
    {
        bool AcquireLock(string resource);
        void ReleaseLock(string resource);
    }
}
