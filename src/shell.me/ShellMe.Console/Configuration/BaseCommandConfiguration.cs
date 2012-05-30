//using System.Collections.Generic;
//using System.Linq;

//namespace ShellMe.Console.Configuration
//{
//    public abstract class BaseCommandConfiguration
//    {
//        private readonly IList<string> _arguments;

//        protected BaseCommandConfiguration(CommandMatcher commandMatcher)
//        {
//            ConfigurationErrors = new List<string>();
//            _arguments = commandMatcher.Arguments;
//            SaveArguments = commandMatcher.SaveArguments;

//            if (_arguments.Select(x => x.ToLower()).Contains("interactive"))
//            {
//                Interactive = true;
//            }
//        }

//        public abstract string Name { get; }

//        public bool IsValid { get; set; }

//        public bool Interactive { protected set; get; }

//        protected IList<string> SaveArguments { get; private set; }

//        public List<string> ConfigurationErrors { get; private set; } 
//    }
//}
