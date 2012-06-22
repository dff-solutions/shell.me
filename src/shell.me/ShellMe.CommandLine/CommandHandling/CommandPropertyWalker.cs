using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ImpromptuInterface;

namespace ShellMe.CommandLine.CommandHandling
{
    public interface ICommandPropertyWalker
    {
        void FillCommandProperties(IEnumerable<string> arguments, ICommand command);
    }

    public class CommandPropertyWalker : ICommandPropertyWalker
    {
        public Dictionary<Type, Func<CommandArgument, object>> TypeProviders { get; private set; } 

        public CommandPropertyWalker()
        {
            TypeProviders = new Dictionary<Type, Func<CommandArgument, object>>();
            
            TypeProviders.Add(typeof(bool), arg => arg.Value == null 
                || arg.Value.Equals("true", StringComparison.OrdinalIgnoreCase)
                || arg.Value.Equals("1", StringComparison.OrdinalIgnoreCase));

            TypeProviders.Add(typeof(string), arg => arg.Value);

            Func<string,int> saveConvertToInt = arg => {
                                                           int value;
                                                           int.TryParse(arg, out value);
                                                           return value;
                                                       };

            TypeProviders.Add(typeof(int), arg => saveConvertToInt(arg.Value));

            TypeProviders.Add(typeof(IEnumerable<int>),arg => arg.Value
                                                                  .Replace("[", "")
                                                                  .Replace("]", "")
                                                                  .Split(',')
                                                                  .Select(saveConvertToInt)
                                                                  .ToList());

            TypeProviders.Add(typeof(IEnumerable<SourceLevels>), arg => arg.Value
                                                                           .Replace("[","")
                                                                           .Replace("]","")
                                                                           .Split(',')
                                                                           .Select(enumString =>
                                                                                       {
                                                                                           SourceLevels level;
                                                                                           Enum.TryParse(enumString, true, out level);
                                                                                           return level;
                                                                                       })
                                                                           .ToList());
        }

        public void FillCommandProperties(IEnumerable<string> arguments, ICommand command)
        {
            foreach (var argument in command.GetCommandProperties())
            {
                if (argument.CanWrite)
                {
                    
                    var arg = GetArgument(arguments, argument.Name);

                    if (arg != null && TypeProviders.ContainsKey(argument.PropertyType))
                    {
                        var value = TypeProviders[argument.PropertyType](arg);
                        Impromptu.InvokeSet(command, argument.Name, value);
                    }
                }
            }
        }

        private CommandArgument GetArgument(IEnumerable<string> arguments, string argument)
        {
            var tempArg = arguments
                    .Select(arg => arg.Trim())
                    .FirstOrDefault(arg => 
                        arg.StartsWith("-") 
                        && arg.Substring(1).Replace("-", "").StartsWith(argument,StringComparison.OrdinalIgnoreCase));

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
