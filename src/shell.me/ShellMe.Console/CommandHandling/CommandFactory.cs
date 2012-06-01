using System;
using System.Collections.Generic;
using System.Linq;

namespace ShellMe.CommandLine.CommandHandling
{
    public class CommandFactory
    {
        private readonly IEnumerable<ICommand> _commands;

        public CommandFactory(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public ICommand GetCommand(string commandName)
        {
            var command = _commands
                            .FirstOrDefault(bundle => bundle.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));

            if (command == null)
                return null;

            //We allways want to return a fresh instance to have an isolated scope (e.g. no properties interfering each other)
            return (ICommand)Activator.CreateInstance(command.GetType());
        }
    }
}
