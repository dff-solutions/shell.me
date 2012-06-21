using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.IO;
using System.Linq;
using System.Reflection;
using ShellMe.CommandLine.Extensions;

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
            //TODO
            //Look into sandboxing the whole thing
            //We currently don't support the case where multiple commands use multiple versions of the same assembly
            //http://stackoverflow.com/questions/4145713/looking-for-a-practical-approach-to-sandboxing-net-plugins

            var pluginDirectory = string.IsNullOrEmpty(path) ? 
                new DirectoryInfo(Path.Combine(Directory.GetParent(Assembly.GetCallingAssembly().Location).FullName, "plugins")) : 
                new DirectoryInfo(path);
            
            //MEF does not default to use the parameterless constructor, so we need to tell it so.
            Func<ConstructorInfo[], ConstructorInfo> constructorFilter = 
                constructorInfos => constructorInfos.FirstOrDefault(
                    constructorInfo => constructorInfo.GetParameters().Length == 0);

            var registration = new RegistrationBuilder();
            registration
                .ForTypesDerivedFrom<ICommand>()
                .Export()
                .Export<ICommand>()
                .SelectConstructor(constructorFilter);

            var directoryCatalogs = pluginDirectory
                .GetDirectories("*", SearchOption.AllDirectories)
                .Select(dir => new DirectoryCatalog(dir.FullName, registration));

            var aggregateCatalog = new AggregateCatalog(directoryCatalogs);

            var container = new CompositionContainer(aggregateCatalog);

            try
            {
                var commands = container.GetExportedValues<ICommand>();
                _commands.AddRange(commands);
            }
            catch (Exception)
            {
                //MEF raises an exception if no plugins were found
            }
        }
    }
}
