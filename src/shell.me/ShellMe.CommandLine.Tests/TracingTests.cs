using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShellMe.CommandLine.CommandHandling;
using ShellMe.Testing;
using ShellMe.CommandLine.Console.LowLevel;

namespace ShellMe.CommandLine.Tests
{
    public class TracingTests
    {
        [Test]
        public void InterpretsEnumerableLogLevelArgumentAssignment()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "LogLevel", "--LogLevel = [Warning,Information] ", "--nonInteractive" });

            Assert.AreEqual("Warning", console.ReadInLineFromTo(7,0,6));
            Assert.AreEqual("Information", console.ReadInLineFromTo(8, 0, 10));
        }

        [Test]
        public void InterpretsEnumerableLogLevelArgumentAssignment2()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "LogLevel", "--LogLevel=[Information, Error]", "--nonInteractive" });

            Assert.AreEqual("Information", console.ReadInLineFromTo(7, 0, 10));
            Assert.AreEqual("Error", console.ReadInLineFromTo(8, 0, 4));
        }

        [Test]
        public void InterpretsEnumerableLogLevelArgumentInInteractiveMode()
        {
            var inputSequence = "LogLevel --logLevel=[Information, Error] --non-interactive".ToInputSequence().AddEnterHit();
            var console = new LowLevelTestConsole(inputSequence);
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new string[]{});

            Assert.AreEqual("(S) LogLevel --logLevel=[Information, Error] --non-interactive", console.ReadInLineFromTo(7, 0, 61));
            Assert.AreEqual("Information", console.ReadInLineFromTo(8, 0, 10));
            Assert.AreEqual("Error", console.ReadInLineFromTo(9, 0, 4));
        }

        //[Test]
        //public void TracesOnlyErrorAndInformationLevel()
        //{
        //    var console = new TestConsole(new List<string>());
        //    var commandFactory = new CommandFactory(new[] { new LogLevelCommand() });
        //    var commandLoop = new CommandLoop(console, commandFactory);
        //    commandLoop.Start(new[] { "LogLevel", "--writeFile=foo.log --LogLevel = [Error,Information] ", "--nonInteractive" });

        //    Assert.AreEqual("Warning", console.OutputQueue[0]);
        //    Assert.AreEqual("Information", console.OutputQueue[1]);
        //}
    }
}
