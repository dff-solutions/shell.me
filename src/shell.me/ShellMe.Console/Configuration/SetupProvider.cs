namespace ShellMe.Console.Configuration
{
    class SetupProvider
    {
        private readonly ConfigurationFactory _configurationFactory;

        public SetupProvider(ConfigurationFactory configurationFactory)
        {
            _configurationFactory = configurationFactory;
        }

        public Setup GetSetup(string[] args)
        {
            var argumentsProvider = new ArgumentsProvider(args);
            var configurationBundle = _configurationFactory.GetConfigurationBundle(argumentsProvider);
            
            if (configurationBundle != null)
            {
                var configuration = configurationBundle.CreateConfiguration(argumentsProvider);
                IProcessor processor = null;
                if (configuration != null)
                {
                    processor = configurationBundle.CreateProcessor(configuration);
                }

                if (configuration != null && processor != null)
                    return new Setup(configuration, processor);
            }
            return null;
        }
    }
}
