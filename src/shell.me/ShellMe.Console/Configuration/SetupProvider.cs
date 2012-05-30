//namespace ShellMe.Console.Configuration
//{
//    class SetupProvider
//    {
//        private readonly CommandFactory _commandFactory;

//        public SetupProvider(CommandFactory commandFactory)
//        {
//            _commandFactory = commandFactory;
//        }

//        public Setup GetSetup(string[] args)
//        {
//            var argumentsProvider = new ArgumentsProvider(args);
//            var configurationBundle = _commandFactory.GetConfigurationBundle(argumentsProvider);
            
//            if (configurationBundle != null)
//            {
//                var configuration = configurationBundle.CreateConfiguration(argumentsProvider);
//                IProcessor processor = null;
//                if (configuration != null)
//                {
//                    processor = configurationBundle.CreateProcessor(configuration);
//                }

//                if (configuration != null && processor != null)
//                    return new Setup(configuration, processor);
//            }
//            return null;
//        }
//    }
//}
