using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShellMe.CommandLine.CommandHandling;
using ShellMe.CommandLine.Console.LowLevel;
using ShellMe.Testing;

namespace ShellMe.CommandLine.Tests
{
    public class CommandTests
    {
        [Test]
        public void CanInitializeCommand()
        {

            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--non-interactive" });

            Assert.AreEqual("Run. Test: False, Text: ", console.ReadInLineFromTo(7, 0,23));
        }

        [Test]
        public void RunningUnknownCommandsDoesNotThrowException()
        {
            var inputSequence = "fooooo".ToInputSequence().AddEnterHit().AddInputSequence("exit").AddEnterHit();
            var console = new LowLevelTestConsole(inputSequence);
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);

            var commandLoop = new CommandLoop(console, commandFactory);
            Assert.DoesNotThrow(() => commandLoop.Start(new[] { "fooo" }));
        }

        [Test]
        public void MaliciousCommandWontTakeShellMeDown()
        {
            var inputSequence = "exit".ToInputSequence().AddEnterHit();
            var console = new LowLevelTestConsole(inputSequence);
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);

            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] {"malicious", " --value=4", "--non-interactive"});
            Assert.AreEqual("Unexpected error happended while proceeding the command: Malicious", console.ReadInLineFromTo(7, 0, 65));
        }

        [Test]
        public void SwitchesToInteractiveModeIfStartedWithoutAnyCommand()
        {
            var inputSequence = "test --nonInteractive".ToInputSequence().AddEnterHit();
            var console = new LowLevelTestConsole(inputSequence);
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new string[]{});

            Assert.AreEqual("Run. Test: False, Text: ", console.ReadInLineFromTo(8, 0, 23));
        }

        [Test]
        public void InterpretsBooleanArgumentWithoutAssignmentAsTrue()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest", "--nonInteractive" });

            Assert.AreEqual("Run. Test: True, Text: ", console.ReadInLineFromTo(7, 0, 22));
        }

        [Test]
        public void InterpretsBooleanArgumentWithFalseAssignmentExpression()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest = false", "--nonInteractive" });

            Assert.AreEqual("Run. Test: False, Text: ", console.ReadInLineFromTo(7, 0, 23));
        }

        [Test]
        public void InterpretsStringArgumentAssignment()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest = false ", "--Text=Foo", "--non-interactive" });

            Assert.AreEqual("Run. Test: False, Text: Foo", console.ReadInLineFromTo(7, 0, 26));
        }

        [Test]
        public void InterpretsIntArgumentAssignment()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "IntProperty", "--Size = 5 ", "--nonInteractive" });

            Assert.AreEqual("5", console.ReadInLineFromTo(7, 0, 0));
        }

        [Test]
        public void InterpretsEnumerableIntArgumentAssignment()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "EnumerableInt", "--Values = [1,2, 3,4] ", "--nonInteractive" });

            Assert.AreEqual("1", console.ReadInLineFromTo(7, 0, 0));
            Assert.AreEqual("2", console.ReadInLineFromTo(8, 0, 0));
            Assert.AreEqual("3", console.ReadInLineFromTo(9, 0, 0));
            Assert.AreEqual("4", console.ReadInLineFromTo(10, 0, 0));
        }

        [Test]
        public void InterpretsEnumerableStringArgumentAssignment()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "EnumerableString", "--Values = [Foo,bar,Foo Bar] ", "--nonInteractive" });

            Assert.AreEqual("Foo", console.ReadInLineFromTo(7, 0, 2));
            Assert.AreEqual("bar", console.ReadInLineFromTo(8, 0, 2));
            Assert.AreEqual("Foo Bar", console.ReadInLineFromTo(9, 0, 6));
        }

        [Test]
        public void IgnoresUnsupportedPropertyTypes()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            Assert.DoesNotThrow(() => commandLoop.Start(new[] { "UnknownProperty", "--Size = {X: 5, Y: 10 } ", "--nonInteractive" }));
        }

        [Test]
        public void RunsTwoTimesInteractiveAndThenClosesAfterLastNonInteractive()
        {
            var inputSequence = "test".ToInputSequence().AddEnterHit().AddInputSequence("test --nonInteractive").AddEnterHit();
            var console = new LowLevelTestConsole(inputSequence);
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest" });

            Assert.AreEqual("Run. Test: True, Text: ", console.ReadInLineFromTo(7, 0, 22));
            Assert.AreEqual("(S) test", console.ReadInLineFromTo(8, 0, 7));
            Assert.AreEqual("Run. Test: False, Text: ", console.ReadInLineFromTo(9, 0, 23));
            Assert.AreEqual("(S) test --nonInteractive", console.ReadInLineFromTo(10, 0, 24));
            Assert.AreEqual("Run. Test: False, Text: ", console.ReadInLineFromTo(11, 0, 23));
            Assert.AreEqual(12, console.BufferLines);
        }

        [Test]
        public void RunsTwoTimesAndThenIgnoresLastCommandBecauseOfPreviousExit()
        {
            var inputSequence = "test".ToInputSequence().AddEnterHit().AddInputSequence("exit").AddEnterHit().AddInputSequence("test").AddEnterHit();

            var console = new LowLevelTestConsole(inputSequence);
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest" });

            Assert.AreEqual("Run. Test: True, Text: ", console.ReadInLineFromTo(7, 0, 22));
            Assert.AreEqual("(S) test", console.ReadInLineFromTo(8, 0, 7));
            Assert.AreEqual("Run. Test: False, Text: ", console.ReadInLineFromTo(9, 0, 23));
            Assert.AreEqual("(S) exit", console.ReadInLineFromTo(10, 0, 7));
            Assert.AreEqual(11, console.BufferLines);
        }

        [Test]
        public void PrintsExceptionsToTheConsole()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "RaiseException", "--nonInteractive" });
            Assert.AreEqual("Unexpected error happended while proceeding the command: RaiseException", console.ReadInLineFromTo(7, 0, 70));
        }

        [Test]
        public void IgnoresSecondCommandBecauseItsConfiguredToNotRunInParallel()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);

            var console2 = new LowLevelTestConsole();
            var commandFactory2 = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop2 = new CommandLoop(console2, commandFactory2);

            Task.WaitAll(new[]
                             {
                                 Task.Factory.StartNew(() => commandLoop.Start(new[] { "LongRunningCommand", "--nonInteractive", "--allow-parallel=false" })),
                                 Task.Factory.StartNew(() => commandLoop2.Start(new[] { "LongRunningCommand", "--nonInteractive", "--allow-parallel=false" }))
                             });

            var successfulRuns = 0;
            var firstCommandRun = console.BufferLines == 8 && console.ReadInLineFromTo(7,0,8) == "Completed";
            var secondCommandRun = console2.BufferLines == 8 && console2.ReadInLineFromTo(7, 0, 8) == "Completed";

            if (firstCommandRun)
                successfulRuns++;

            if (secondCommandRun)
                successfulRuns++;

            Assert.AreEqual(1, successfulRuns);
        }

        [Test]
        public void RunsCommandsInParallel()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);

            var console2 = new LowLevelTestConsole();
            var commandFactory2 = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop2 = new CommandLoop(console2, commandFactory2);

            Task.WaitAll(new[]
                             {
                                 Task.Factory.StartNew(() => commandLoop.Start(new[] { "LongRunningCommand", "--nonInteractive", "--allow-parallel=true" })),
                                 Task.Factory.StartNew(() => commandLoop2.Start(new[] { "LongRunningCommand", "--nonInteractive", "--allow-parallel=true" }))
                             });

            Assert.IsTrue(console.ReadInLineFromTo(7, 0, 8) == "Completed");
            Assert.IsTrue(console2.ReadInLineFromTo(7, 0, 8) == "Completed");
        }

        [Test]
        public void RunsCommandsInParallelBecauseAllowParallelIsDefaultedToTrue()
        {
            var console = new LowLevelTestConsole();
            var commandFactory = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop = new CommandLoop(console, commandFactory);

            var console2 = new LowLevelTestConsole();
            var commandFactory2 = new CommandFactory(Configurations.PluginDirectory);
            var commandLoop2 = new CommandLoop(console2, commandFactory2);

            Task.WaitAll(new[]
                             {
                                 Task.Factory.StartNew(() => commandLoop.Start(new[] { "LongRunningCommand", "--nonInteractive" })),
                                 Task.Factory.StartNew(() => commandLoop2.Start(new[] { "LongRunningCommand", "--nonInteractive" }))
                             });

            Assert.IsTrue(console.ReadInLineFromTo(7, 0, 8) == "Completed");
            Assert.IsTrue(console2.ReadInLineFromTo(7, 0, 8) == "Completed");
        }
    }
}
