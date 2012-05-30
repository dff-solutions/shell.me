using System.Collections.Generic;
using System.Linq;

namespace ShellMe.Console.Configuration
{
    public class ArgumentsProvider
    {
        public IList<string> SaveArguments { get; private set; }
        public IList<string> Arguments { get; private set; }
        public Dictionary<string, string> KeyValueArguments { get; set; }
        public string CommandName { get; set; }

        public ArgumentsProvider(IEnumerable<string> arguments)
        {
            CommandName = arguments == null || !arguments.Any() ? string.Empty : arguments.First().ToLower().Trim();

            Arguments = arguments == null ? Enumerable.Empty<string>().ToList() : arguments.Select(x => x.Trim()).ToList();

            SaveArguments = Arguments.Where(x => x.ToLower() != "interactive").ToList();
        }
    }
}
