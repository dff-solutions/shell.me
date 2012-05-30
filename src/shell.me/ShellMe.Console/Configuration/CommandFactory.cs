using System;
using System.Collections.Generic;
using System.Linq;

namespace ShellMe.Console.Configuration
{
    public class CommandFactory
    {
        private readonly IEnumerable<ICommand> _commands;

        public CommandFactory(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public ICommand GetCommand(ArgumentsProvider argumentsProvider)
        {
            var command = _commands
                            .FirstOrDefault(bundle => bundle.Name.Equals(argumentsProvider.CommandName, StringComparison.OrdinalIgnoreCase));

            return command;
        }
    }
}
