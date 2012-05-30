using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ShellMe.Console.Configuration;
using ShellMe.Testing;

namespace ShellMe.Console.Tests
{
    public class CommandInitTests
    {
        [Test]
        public void CanInitializeCommand()
        {
            var console = new TestConsole(new List<string>() {"--test"});
            var commandFactory = new CommandFactory(new[] {new TestCommand()});
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "--test" });

            Assert.AreEqual("Unexpected error happended while proceeding the command: Test", console.OutputQueue[0]);
        }
    }
}
