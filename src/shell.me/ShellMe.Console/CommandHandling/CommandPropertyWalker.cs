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
        public Dictionary<string, Func<string, object>> TypeProviders { get; private set; } 

        public CommandPropertyWalker()
        {
            TypeProviders = new Dictionary<string, Func<string, object>>();
            
            //Todo This does not deal with the case where someone actually says --foo=false
            TypeProviders.Add("System.Boolean", arg => !string.IsNullOrEmpty(arg));
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

        private string GetArgument(IEnumerable<string> arguments, string argument)
        {
            return arguments
                    .Select(arg => arg.Trim())
                    .FirstOrDefault(arg => arg.Equals("--" + argument, StringComparison.OrdinalIgnoreCase));
        }
    }
}
