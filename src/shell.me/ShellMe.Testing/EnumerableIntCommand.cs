using System.Collections.Generic;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.Testing
{
    public class EnumerableIntPropertyCommand : BaseCommand
    {
        public override string Name
        {
            get { return "EnumerableInt"; }
        }

        public override void Run()
        {
            foreach (var i in Values)
            {
                Console.WriteLine(i.ToString());
            }
        }

        public IEnumerable<int> Values { get; set; }
    }
}
