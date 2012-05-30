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

            Assert.AreEqual("Run", console.OutputQueue[0]);
        }

        [Test]
        public void SetsBooleanCommandProperties()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(new[] { new TestCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "--test", "--IsTest" });

            Assert.AreEqual("Run with Test", console.OutputQueue[0]);
        }

        [Test]
        public void RunsIndefinetlyInInteractiveMode()
        {
            var console = new TestConsole(new List<string>() { "--test", "exit" });
            var commandFactory = new CommandFactory(new[] { new TestCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "--test", "--IsTest", "--interactive" });

            Assert.AreEqual("Run with Test", console.OutputQueue[0]);
            Assert.AreEqual("Run", console.OutputQueue[2]);
            Assert.AreEqual(4, console.OutputQueue.Count);
        }
    }
}
