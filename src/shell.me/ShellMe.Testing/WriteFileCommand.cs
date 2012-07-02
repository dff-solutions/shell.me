using System.Collections.Generic;
using System.IO;
using System.Net;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.Testing
{
    public class WriteFileCommand : BaseCommand
    {
        public override string Name
        {
            get { return "WriteFile"; }
        }

        public override void Run()
        {
            using (var stream = File.CreateText(FileName))
            {
                stream.WriteLine("");
            }
        }

        public string FileName { get; set; }
    }
}
