using System.Collections.Generic;
using System.Reflection;
using ShellMe.Console.Configuration;

namespace ShellMe.Console.CommandHandling
{
    public static class CommandExtensions
    {
        public static IEnumerable<PropertyInfo> GetCommandProperties(this ICommand command)
        {
            return command.GetType().GetProperties();
        } 
    }
}
