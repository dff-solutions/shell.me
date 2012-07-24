using System;

namespace ShellMe.CommandLine.Locking
{
    public interface ILockingService
    {
        bool AcquireLock(string resource);
        void ReleaseLock(string resource);
    }
}
