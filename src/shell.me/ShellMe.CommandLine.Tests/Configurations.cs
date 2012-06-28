using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine.Tests
{
    class Configurations
    {
        public static string PluginDirectory
        {
            get { return Path.Combine(Environment.CurrentDirectory, "plugins"); }
        }
    }
}
