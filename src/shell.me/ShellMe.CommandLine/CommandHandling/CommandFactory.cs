using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ShellMe.CommandLine.CommandHandling
{
    public class CommandFactory
    {
        private readonly List<ICommand> _commands;

        public CommandFactory(IEnumerable<ICommand> commands):this(commands, null)
        {
            
        }

        public CommandFactory(IEnumerable<ICommand> commands, string pluginDirectory)
        {
            _commands = new List<ICommand>(commands);
            LoadCommands(pluginDirectory);
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

        private void LoadCommands(string path)
        {
            var pluginDirectory = string.IsNullOrEmpty(path) ? 
                Directory.GetParent(Assembly.GetCallingAssembly().Location) : 
                new DirectoryInfo(path);

            foreach (var fileInfo in pluginDirectory.GetFiles("*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFile(fileInfo.FullName);
                    _commands.AddRange(GetCommandsFromAssembly(assembly));
                }
                catch (Exception)
                {
                    //Just skip any non .NET dlls
                }
            }
        }

        private IEnumerable<ICommand> GetCommandsFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly
                    .GetTypes()
                    .Where(type =>
                               {
                                   return type.IsClass;
                               })
                    .Where(type =>
                               {
                                   return type.GetInterface("ShellMe.CommandLine.CommandHandling.ICommand") != null;
                               })
                    .Select(type =>
                                {
                                    try
                                    {
                                        return (ICommand) assembly.CreateInstance(type.ToString());
                                    }
                                    catch (Exception)
                                    {
                                        return null;
                                    }
                                })
                    .Where(command => command != null);
            }
            catch (Exception exception)
            {
                return Enumerable.Empty<ICommand>();
            }
        } 
    }
}
