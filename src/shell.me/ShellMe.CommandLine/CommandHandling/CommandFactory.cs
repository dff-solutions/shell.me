using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ShellMe.CommandLine.CommandHandling
{
    public class CommandMetaData
    {
        public ICommand Command { get; set; }
        public AppDomain Domain { get; set; }
        public string AssemblyName { get; set; }
        public string TypeName { get; set; }
    }

    public class CommandFactory
    {
        private readonly List<CommandMetaData> _commandMetaDataList; 

        public CommandFactory():this(null)
        {
        }

        public CommandFactory(string pluginDirectory)
        {
            _commandMetaDataList = new List<CommandMetaData>();
            LoadCommands(pluginDirectory);
        }

        public ICommand GetCommand(string commandName)
        {
            var commandMetaData = _commandMetaDataList
                            .FirstOrDefault(bundle => bundle.Command.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));

            //We allways want to return a fresh instance to have an isolated scope (e.g. no properties interfering each other)
            return commandMetaData == null ? null : CreateCommand(commandMetaData);
        }

        private void LoadCommands(string path)
        {
            var pluginDirectory = string.IsNullOrEmpty(path) ?
                new DirectoryInfo(Path.Combine(Directory.GetParent(Assembly.GetCallingAssembly().Location).FullName, "plugins")) :
                new DirectoryInfo(path);

            _commandMetaDataList.AddRange(
                pluginDirectory
                    .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                    .SelectMany(LoadCommandsFromDirectory));
        }

        public IEnumerable<CommandMetaData> LoadCommandsFromDirectory(DirectoryInfo directory)
        {
            AppDomain domain = null;
            var commands = new List<CommandMetaData>();
            foreach (var file in directory.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file.FullName);

                    foreach (var type in assembly.GetExportedTypes())
                    {
                        if (type.GetInterface("ShellMe.CommandLine.CommandHandling.ICommand") != null && !type.IsAbstract)
                        {
                            if (domain == null)
                            {
                                var domainSetup = new AppDomainSetup()
                                                      {
                                                          ApplicationBase = directory.FullName
                                                      };
                                domain = AppDomain.CreateDomain("Sandbox Domain", null, domainSetup);
                            }

                            var assemblyName = assembly.FullName;
                            var typeName = type.FullName;

                            var metaData = new CommandMetaData()
                            {
                                Domain = domain,
                                AssemblyName = assemblyName,
                                TypeName = typeName
                            };

                            var command = CreateCommand(metaData);
                            metaData.Command = command;

                            commands.Add(metaData);
                        }
                    }
                }
                catch (Exception e)
                {
                    // Ignore DLLs that are not .NET assemblies.
                }
            }
            return commands;
        }

        public ICommand CreateCommand(CommandMetaData commandMetaData)
        {
            return (ICommand)commandMetaData.Domain.CreateInstanceAndUnwrap(commandMetaData.AssemblyName, commandMetaData.TypeName);
        }
    }
}
