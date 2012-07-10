using System.Collections.Generic;

namespace ShellMe.CommandLine.CommandHandling
{
    public interface ICommandFactory
    {
        IEnumerable<ICommand> GetAvailable();
        ICommand GetCommand(string commandName);
    }
}