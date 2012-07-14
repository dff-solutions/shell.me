using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShellMe.CommandLine.CommandHandling;
using ShellMe.Testing;

namespace ShellMe.CommandLine.Tests
{
    public class CommandTests
    {
        [Test]
        public void CanInitializeCommand()
        {
            var console = new TestConsole(new List<string>() {});
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--non-interactive" });

            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[7]);
        }

        [Test]
        public void RunningUnknownCommandsDoesNotThrowException()
        {
            var console = new TestConsole(new List<string>() {"--test", "exit" });
            //var commandFactory = new CommandFactory(new[] {new TestCommand()});
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);

            var commandLoop = new CommandLoop(console, commandFactory);
            Assert.DoesNotThrow(() => commandLoop.Start(new[] { "--test" }));
        }

        [Test]
        public void MaliciousCommandWontTakeShellMeDown()
        {
            var console = new TestConsole(new List<string>(){"exit"});
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);

            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] {"malicious", " --value=4", "--non-interactive"});
            Assert.True(console.OutputQueue[7].StartsWith("Unexpected error happended while proceeding the command: Malicious"));
            
        }

        [Test]
        public void SwitchesToInteractiveModeIfStartedWithoutAnyCommand()
        {
            var console = new TestConsole(new List<string>() { "test --nonInteractive" });
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new string[]{});

            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[7]);
        }

        [Test]
        public void InterpretsBooleanArgumentWithoutAssignmentAsTrue()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest", "--nonInteractive" });

            Assert.IsTrue(console.OutputQueue.Exists(x => x == "Run. Test: True, Text: "));
        }

        [Test]
        public void InterpretsBooleanArgumentWithFalseAssignmentExpression()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest = false", "--nonInteractive" });

            Assert.IsTrue(console.OutputQueue.Exists(x => x == "Run. Test: False, Text: "));
        }

        [Test]
        public void InterpretsStringArgumentAssignment()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest = false ", "--Text=Foo", "--non-interactive" });

            Assert.IsTrue(console.OutputQueue.Exists(x => x == "Run. Test: False, Text: Foo"));
        }

        [Test]
        public void InterpretsIntArgumentAssignment()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "IntProperty", "--Size = 5 ", "--nonInteractive" });

            Assert.IsTrue(console.OutputQueue.Exists(x => x == "5"));
        }

        [Test]
        public void InterpretsEnumerableIntArgumentAssignment()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "EnumerableInt", "--Values = [1,2, 3,4] ", "--nonInteractive" });

            Assert.AreEqual("1", console.OutputQueue[7]);
            Assert.AreEqual("2", console.OutputQueue[8]);
            Assert.AreEqual("3", console.OutputQueue[9]);
            Assert.AreEqual("4", console.OutputQueue[10]);
        }

        [Test]
        public void IgnoresUnsupportedPropertyTypes()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            Assert.DoesNotThrow(() => commandLoop.Start(new[] { "UnknownProperty", "--Size = {X: 5, Y: 10 } ", "--nonInteractive" }));
        }

        [Test]
        public void RunsTwoTimesInteractiveAndThenClosesAfterLastNonInteractive()
        {
            var console = new TestConsole(new List<string>() { "test", "test --nonInteractive" });
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest" });

            Assert.AreEqual("Run. Test: True, Text: ", console.OutputQueue[7]);
            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[8]);
            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[9]);
            Assert.AreEqual(10, console.OutputQueue.Count);
        }

        [Test]
        public void RunsTwoTimesInteractiveAndThenIgnoresLastCommandBecauseOfPreviousExit()
        {
            var console = new TestConsole(new List<string>() { "test", "exit", "--test" });
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest" });

            Assert.AreEqual("Run. Test: True, Text: ", console.OutputQueue[7]);
            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[8]);
            Assert.AreEqual(9, console.OutputQueue.Count);
        }

        [Test]
        public void PrintsExceptionsToTheConsole()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "RaiseException", "--nonInteractive" });
            Assert.IsTrue(console.OutputQueue[7].StartsWith("Unexpected error happended while proceeding the command: RaiseException"));
        }

        [Test]
        public void IgnoresSecondCommandBecauseItsConfiguredToNotRunInParallel()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);

            var console2 = new TestConsole(new List<string>());
            var commandFactory2 = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop2 = new CommandLoop(console2, commandFactory2);

            Task.WaitAll(new[]
                             {
                                 Task.Factory.StartNew(() => commandLoop.Start(new[] { "LongRunningCommand", "--nonInteractive", "--allow-parallel=false" })),
                                 Task.Factory.StartNew(() => commandLoop2.Start(new[] { "LongRunningCommand", "--nonInteractive", "--allow-parallel=false" }))
                             });

            var successfulRuns = 0;
            var firstCommandRun = console.OutputQueue.Exists(x => x == "Completed");
            var secondCommandRun = console2.OutputQueue.Exists(x => x == "Completed");

            if (firstCommandRun)
                successfulRuns++;

            if (secondCommandRun)
                successfulRuns++;

            Assert.AreEqual(1, successfulRuns);
        }

        [Test]
        public void RunsCommandsInParallel()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);

            var console2 = new TestConsole(new List<string>());
            var commandFactory2 = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop2 = new CommandLoop(console2, commandFactory2);

            Task.WaitAll(new[]
                             {
                                 Task.Factory.StartNew(() => commandLoop.Start(new[] { "LongRunningCommand", "--nonInteractive", "--allow-parallel=true" })),
                                 Task.Factory.StartNew(() => commandLoop2.Start(new[] { "LongRunningCommand", "--nonInteractive", "--allow-parallel=true" }))
                             });

            Assert.IsTrue(console.OutputQueue.Exists(x => x == "Completed"));
            Assert.IsTrue(console2.OutputQueue.Exists(x => x == "Completed"));
        }

        [Test]
        public void RunsCommandsInParallelBecauseAllowParallelIsDefaultedToTrue()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);

            var console2 = new TestConsole(new List<string>());
            var commandFactory2 = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop2 = new CommandLoop(console2, commandFactory2);

            Task.WaitAll(new[]
                             {
                                 Task.Factory.StartNew(() => commandLoop.Start(new[] { "LongRunningCommand", "--nonInteractive" })),
                                 Task.Factory.StartNew(() => commandLoop2.Start(new[] { "LongRunningCommand", "--nonInteractive" }))
                             });

            Assert.IsTrue(console.OutputQueue.Exists(x => x == "Completed"));
            Assert.IsTrue(console2.OutputQueue.Exists(x => x == "Completed"));
        }
    }
}
