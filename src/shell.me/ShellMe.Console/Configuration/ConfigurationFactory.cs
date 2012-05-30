using System.Collections.Generic;
using System.Linq;

namespace ShellMe.Console.Configuration
{
    public class ConfigurationFactory
    {
        private readonly IEnumerable<ICommandBundle> _configurationBundles;

        public ConfigurationFactory(IEnumerable<ICommandBundle> configurationBundles)
        {
            _configurationBundles = configurationBundles;
        }

        public ICommandBundle GetConfigurationBundle(ArgumentsProvider argumentsProvider)
        {
            var matchedConfiguration = _configurationBundles
                                            .FirstOrDefault(bundle => bundle.CommandName.ToLower().Trim() == argumentsProvider.CommandName);

            return matchedConfiguration;
        }
    }
}
