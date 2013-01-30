using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.Testing
{

    public class EnumerableStringCommand : BaseCommand
    {
        public override string Name
        {
            get { return "EnumerableString"; }
        }

        public override void Run()
        {
            foreach (var i in Values)
            {
                Console.WriteLine(i);
            }
        }

        public IEnumerable<string> Values { get; set; }
    }
}
