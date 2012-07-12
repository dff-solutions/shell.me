using System;

namespace ShellMe.CommandLine.Console
{
    public abstract class AbstractConsole : MarshalByRefObject
    {
        public abstract void WriteLine(string line);
        public abstract string ReadLine();
        public abstract ConsoleColor ForegroundColor { get; set; }

        public override object InitializeLifetimeService()
        {
            // returning null here will prevent the lease manager
            // from deleting the object.
            return null;
        }
    }
}
