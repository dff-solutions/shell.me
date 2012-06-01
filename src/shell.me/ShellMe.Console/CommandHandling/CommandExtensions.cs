using System.Collections.Generic;
using System.Reflection;

namespace ShellMe.CommandLine.CommandHandling
{
    public static class CommandExtensions
    {
        public static IEnumerable<PropertyInfo> GetCommandProperties(this ICommand command)
        {
            return command.GetType().GetProperties();
        } 
    }
}
