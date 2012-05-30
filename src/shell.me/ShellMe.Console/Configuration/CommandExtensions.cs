using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ShellMe.Console.Configuration
{
    public static class CommandExtensions
    {
        public static IEnumerable<PropertyInfo> GetCommandProperties(this ICommand command)
        {
            return command.GetType().GetProperties();
        } 
    }
}
