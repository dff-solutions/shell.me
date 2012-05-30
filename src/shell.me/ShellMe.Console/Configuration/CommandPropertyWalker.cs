using System.Collections.Generic;
using System.Linq;
using ImpromptuInterface;

namespace ShellMe.Console.Configuration
{
    public class CommandPropertyWalker
    {
        public void FillCommandProperties(IEnumerable<string> arguments, ICommand command)
        {
            if (ContainsArgument(arguments, "interactive"))
                command.Interactive = true;

            foreach (var argument in command.GetCommandProperties())
            {
                if (argument.CanWrite)
                {
                    if (argument.PropertyType.FullName == "System.Boolean")
                    {
                        //Todo This does not deal with the case where someone actually says --foo=false
                        if (ContainsArgument(arguments, argument.Name))
                        {
                            Impromptu.InvokeSet(command, argument.Name, true);
                        }
                    }
                }
            }
        }

        private bool ContainsArgument(IEnumerable<string> arguments, string argument)
        {
            return (arguments.Select(arg => arg.Trim()).Contains("--" + argument));
        }
    }
}
