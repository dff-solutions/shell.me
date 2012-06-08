using System.Collections.Generic;
using NUnit.Framework;
using ShellMe.CommandLine.CommandHandling;
using ShellMe.Testing;

namespace ShellMe.CommandLine.Tests
{
    public class CommandInitTests
    {
        [Test]
        public void CanInitializeCommand()
        {
            var console = new TestConsole(new List<string>() {});
            var commandFactory = new CommandFactory(new[] {new TestCommand()});
            
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--non-interactive" });

            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[0]);
        }

        [Test]
        public void RunningUnknownCommandsDoesNotThrowException()
        {
            var console = new TestConsole(new List<string>() {"--test", "exit" });
            //var commandFactory = new CommandFactory(new[] {new TestCommand()});
            var commandFactory = new CommandFactory(new ICommand[] { });

            var commandLoop = new CommandLoop(console, commandFactory);
            Assert.DoesNotThrow(() => commandLoop.Start(new[] { "--test" }));
        }

        [Test]
        public void SwitchesToInteractiveModeIfStartedWithoutAnyCommand()
        {
            var console = new TestConsole(new List<string>() { "test --nonInteractive" });
            var commandFactory = new CommandFactory(new[] { new TestCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new string[]{});

            Assert.AreEqual("Enter commands or type exit to close", console.OutputQueue[0]);
            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[1]);
        }

        [Test]
        public void InterpretsBooleanArgumentWithoutAssignmentAsTrue()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(new[] { new TestCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest", "--nonInteractive" });

            Assert.AreEqual("Run. Test: True, Text: ", console.OutputQueue[0]);
        }

        [Test]
        public void InterpretsBooleanArgumentWithFalseAssignmentExpression()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(new[] { new TestCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest = false", "--nonInteractive" });

            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[0]);
        }

        [Test]
        public void InterpretsStringArgumentAssignment()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(new[] { new TestCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest = false ", "--Text=Foo", "--non-interactive" });

            Assert.AreEqual("Run. Test: False, Text: Foo", console.OutputQueue[0]);
        }

        [Test]
        public void InterpretsIntArgumentAssignment()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(new[] { new IntPropertyCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--Size = 5 ", "--nonInteractive" });

            Assert.AreEqual("5", console.OutputQueue[0]);
        }

        [Test]
        public void IgnoresUnsupportedPropertyTypes()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(new[] { new UnknownPropertyCommand(),  });
            var commandLoop = new CommandLoop(console, commandFactory);
            Assert.DoesNotThrow(() => commandLoop.Start(new[] { "test", "--Size = {X: 5, Y: 10 } ", "--nonInteractive" }));
        }

        [Test]
        public void RunsTwoTimesInteractiveAndThenClosesAfterLastNonInteractive()
        {
            var console = new TestConsole(new List<string>() { "test", "test --nonInteractive" });
            var commandFactory = new CommandFactory(new[] { new TestCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest" });

            Assert.AreEqual("Run. Test: True, Text: ", console.OutputQueue[0]);
            Assert.AreEqual("Enter commands or type exit to close", console.OutputQueue[1]);
            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[2]);
            Assert.AreEqual("Enter commands or type exit to close", console.OutputQueue[3]);
            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[4]);
            Assert.AreEqual(5, console.OutputQueue.Count);
        }

        [Test]
        public void RunsTwoTimesInteractiveAndThenIgnoresLastCommandBecauseOfPreviousExit()
        {
            var console = new TestConsole(new List<string>() { "test", "exit", "--test" });
            var commandFactory = new CommandFactory(new[] { new TestCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "test", "--IsTest" });

            Assert.AreEqual("Run. Test: True, Text: ", console.OutputQueue[0]);
            Assert.AreEqual("Enter commands or type exit to close", console.OutputQueue[1]);
            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[2]);
            Assert.AreEqual("Enter commands or type exit to close", console.OutputQueue[3]);
            Assert.AreEqual(4, console.OutputQueue.Count);
        }

        [Test]
        public void PrintsExceptionsToTheConsole()
        {
            var console = new TestConsole(new List<string>());
            var commandFactory = new CommandFactory(new[] { new ExceptionCommand() });
            var commandLoop = new CommandLoop(console, commandFactory);
            commandLoop.Start(new[] { "RaiseException", "--nonInteractive" });

            Assert.AreEqual("Unexpected error happended while proceeding the command: RaiseException", console.OutputQueue[0]);
            Assert.AreEqual("Exception: Foo", console.OutputQueue[1]);
            Assert.IsTrue(console.OutputQueue[2].StartsWith("Stacktrace:"));
            Assert.AreEqual("Exception: Bar", console.OutputQueue[3]);
        }

        [Test]
        public void ReturnsLastCommandOnArrowUp()
        {
            //System.ConsoleKey.UpArrow.ToString() represented as "<" in tests
            var console = new TestConsole(new List<string>() { "test", "<", "exit" });
            var commandFactory = new CommandFactory(new[] { new TestCommand() });
            var commandLoop = new CommandLoop(console, commandFactory,new TestCommandHistory(console) );
            commandLoop.Start(new[] { "test", "<", "exit" });

            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[0]);
            Assert.AreEqual("Enter commands or type exit to close", console.OutputQueue[1]);
            Assert.AreEqual("Run. Test: False, Text: ", console.OutputQueue[2]);
            Assert.AreEqual("Enter commands or type exit to close", console.OutputQueue[3]);
            Assert.AreEqual("test", console.OutputQueue[4]);
            Assert.AreEqual(5, console.OutputQueue.Count);



        }
    }
}
