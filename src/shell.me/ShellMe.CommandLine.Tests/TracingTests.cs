using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShellMe.CommandLine.CommandHandling;
using ShellMe.Testing;

namespace ShellMe.CommandLine.Tests
{
    public class TracingTests
    {
        [Test]
        public void InterpretsEnumerableLogLevelArgumentAssignment()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(new[] { new LogLevelCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "LogLevel", "--LogLevel = [Warning,Information] ", "--nonInteractive" });

            Assert.AreEqual("Warning", console.OutputQueue[0]);
            Assert.AreEqual("Information", console.OutputQueue[1]);
        }

        [Test]
        public void InterpretsEnumerableLogLevelArgumentAssignment2()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(new[] { new LogLevelCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "LogLevel", "--LogLevel=[Information, Error]", "--nonInteractive" });

            Assert.AreEqual("Information", console.OutputQueue[0]);
            Assert.AreEqual("Error", console.OutputQueue[1]);
        }

        [Test]
        public void InterpretsEnumerableLogLevelArgumentInInteractiveMode()
        {
            var console = new TestConsole(new List<string>(){"LogLevel --logLevel=[Information, Error] --non-interactive"});
            var commandFactory = new CommandFactory(new[] { new LogLevelCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new string[]{});

            Assert.AreEqual("Information", console.OutputQueue[0]);
            Assert.AreEqual("Error", console.OutputQueue[1]);
        }

        [Test]
        public void TracesOnlyErrorAndInformationLevel()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(new[] { new LogLevelCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "LogLevel", "--writeFile=foo.log --LogLevel = [Error,Information] ", "--nonInteractive" });

            Assert.AreEqual("Warning", console.OutputQueue[0]);
            Assert.AreEqual("Information", console.OutputQueue[1]);
        }
    }
}
