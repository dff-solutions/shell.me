using System;
using System.Collections.Generic;
using System.Linq;
using ImpromptuInterface;
using ShellMe.Console.Configuration;

namespace ShellMe.Console.CommandHandling
{
    public interface ICommandPropertyWalker
    {
        void FillCommandProperties(IEnumerable<string> arguments, ICommand command);
    }

    public class CommandPropertyWalker : ICommandPropertyWalker
    {
        public Dictionary<string, Func<CommandArgument, object>> TypeProviders { get; private set; } 

        public CommandPropertyWalker()
        {
            TypeProviders = new Dictionary<string, Func<CommandArgument, object>>();
            
            TypeProviders.Add("System.Boolean", arg => arg.Value == null 
                || arg.Value.Equals("true", StringComparison.OrdinalIgnoreCase)
                || arg.Value.Equals("1", StringComparison.OrdinalIgnoreCase));
        }

        public void FillCommandProperties(IEnumerable<string> arguments, ICommand command)
        {
            if (ContainsArgument(arguments, "interactive"))
                command.Interactive = true;

            foreach (var argument in command.GetCommandProperties())
            {
                if (argument.CanWrite)
                {
                    
                    var arg = GetArgument(arguments, argument.Name);

                    if (arg != null && TypeProviders[argument.PropertyType.FullName] != null)
                    {
                        var value = TypeProviders[argument.PropertyType.FullName](arg);
                        Impromptu.InvokeSet(command, argument.Name, value);
                    }
                }
            }
        }

        private bool ContainsArgument(IEnumerable<string> arguments, string argument)
        {
            return arguments.Select(arg => arg.Trim()).Contains("--" + argument);
        }

        private CommandArgument GetArgument(IEnumerable<string> arguments, string argument)
        {
            var tempArg = arguments
                    .Select(arg => arg.Trim())
                    .FirstOrDefault(arg => arg.StartsWith("--" + argument, StringComparison.OrdinalIgnoreCase));

            if (tempArg == null)
                return null;

            var splittedArg = tempArg.Split('=');

            return new CommandArgument()
                       {
                           Name = splittedArg.Length == 0 ? tempArg.Trim() : splittedArg[0].Trim(),
                           Value = splittedArg.Length == 2 ? splittedArg[1].Trim() : null
                       };
        }
    }
}
