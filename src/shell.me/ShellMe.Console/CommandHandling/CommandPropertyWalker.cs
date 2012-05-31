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

            TypeProviders.Add("System.String", arg => arg.Value);

            TypeProviders.Add("System.Int32", arg =>
                                                  {
                                                      int value;
                                                      int.TryParse(arg.Value, out value);
                                                      return value;
                                                  });
        }

        public void FillCommandProperties(IEnumerable<string> arguments, ICommand command)
        {
            foreach (var argument in command.GetCommandProperties())
            {
                if (argument.CanWrite)
                {
                    
                    var arg = GetArgument(arguments, argument.Name);

                    if (arg != null && TypeProviders.ContainsKey(argument.PropertyType.FullName))
                    {
                        var value = TypeProviders[argument.PropertyType.FullName](arg);
                        Impromptu.InvokeSet(command, argument.Name, value);
                    }
                }
            }
        }

        private CommandArgument GetArgument(IEnumerable<string> arguments, string argument)
        {
            var tempArg = arguments
                    .Select(arg => arg.Trim())
                    .FirstOrDefault(arg => arg.StartsWith("--" + argument, StringComparison.OrdinalIgnoreCase));

            if (tempArg == null)
                return null;

            var splittedArg = tempArg.Split('=');

            Func<string, string> saveTrim = str => !string.IsNullOrEmpty(str) ? str.Trim() : str;

            return new CommandArgument()
                       {
                           Name = saveTrim(splittedArg.Length == 0 ? tempArg : splittedArg[0]),
                           Value = saveTrim(splittedArg.Length == 2 ? splittedArg[1] : null)
                       };
        }
    }
}
