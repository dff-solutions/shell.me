using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ShellMe.CommandLine.Console.LowLevel;
using ShellMe.Testing;

namespace ShellMe.CommandLine.Tests
{
    [TestFixture]
    class LowLevelToAbstractConsoleAdapterTests
    {
        [Test]
        public void WritesPrompt()
        {
            var inputSequence = " ".ToInputSequence();
            var lowLevelConsole = new LowLevelTestConsole(inputSequence);
            var adapter = new LowLevelToAbstractConsoleAdapter(lowLevelConsole) {Prompt = "$ "};

            adapter.ReadUntilSequenceIsOver(inputSequence);
            var written = lowLevelConsole.ReadCharacterAt(0, 0);
            Assert.AreEqual('$',written);
        }

        [Test]
        public void WritesHelloWorld()
        {
            var inputSequence = "Hello World".ToInputSequence();
            var lowLevelConsole = new LowLevelTestConsole(inputSequence);
            var adapter = new LowLevelToAbstractConsoleAdapter(lowLevelConsole) { Prompt = "$ " };

            adapter.ReadUntilSequenceIsOver(inputSequence);

            Assert.AreEqual("$ Hello World", lowLevelConsole.ReadInLineFromTo(0,0,12));
        }

        [Test]
        public void BreaksLineOnTheRightCharacter()
        {
            var inputSequence = "Hello this is shell.me. We are testing the new LowLevelConsole, which gives us great power!".ToInputSequence();
            var lowLevelConsole = new LowLevelTestConsole(inputSequence);
            var adapter = new LowLevelToAbstractConsoleAdapter(lowLevelConsole) { Prompt = "(S) " };

            adapter.ReadUntilSequenceIsOver(inputSequence);

            Assert.AreEqual("(S) Hello this is shell.me. We are testing the new LowLevelConsole, which gives ", lowLevelConsole.ReadInLineFromTo(0, 0, 79));
            Assert.AreEqual("us great power!", lowLevelConsole.ReadInLineFromTo(1, 0, 14));
        }

        [Test]
        public void BackspaceJumpsLineUpAndLetsUsWriteAgain()
        {
            var inputSequence = "Hello this is shell.me. We are testing the new LowLevelConsole, which gives us great power!"
                .ToInputSequence()
                .AddBackspaceHit(21)
                .AddInputSequence("rocks!");

            var lowLevelConsole = new LowLevelTestConsole(inputSequence);
            var adapter = new LowLevelToAbstractConsoleAdapter(lowLevelConsole) { Prompt = "(S) " };

            adapter.ReadUntilSequenceIsOver(inputSequence);

            Assert.AreEqual("(S) Hello this is shell.me. We are testing the new LowLevelConsole, which rocks!", lowLevelConsole.ReadInLineFromTo(0, 0, 79));
            //We are just making sure that there is nothing left on the second line
            Assert.AreEqual("                                                                                ", lowLevelConsole.ReadInLineFromTo(1, 0, 79));
        }

        [Test]
        public void CursorMovesLeftUntilItHitsThePrompt()
        {
            var inputSequence = "Hello"
                .ToInputSequence()
                .AddLeftArrowHit(5);

            var lowLevelConsole = new LowLevelTestConsole(inputSequence);
            var adapter = new LowLevelToAbstractConsoleAdapter(lowLevelConsole) { Prompt = "(S) " };

            adapter.ReadUntilSequenceIsOver(inputSequence);

            Assert.AreEqual("(S) Hello", lowLevelConsole.ReadInLineFromTo(0, 0, 8));
            Assert.AreEqual(4, lowLevelConsole.CursorLeft);
            Assert.AreEqual(0, lowLevelConsole.CursorTop);
        }

        [Test]
        public void CursorCanNotMoveAheadTheWriting()
        {
            var inputSequence = "Hello"
                .ToInputSequence()
                .AddRightArrowHit(5);

            var lowLevelConsole = new LowLevelTestConsole(inputSequence);
            var adapter = new LowLevelToAbstractConsoleAdapter(lowLevelConsole) { Prompt = "(S) " };

            adapter.ReadUntilSequenceIsOver(inputSequence);

            Assert.AreEqual("(S) Hello", lowLevelConsole.ReadInLineFromTo(0, 0, 8));
            Assert.AreEqual(9, lowLevelConsole.CursorLeft);
            Assert.AreEqual(0, lowLevelConsole.CursorTop);
        }

        [Test]
        public void WritesMovesCursorBeforeWritingAndInsertsAnotherWord()
        {
            var inputSequence = "World"
                .ToInputSequence()
                .AddLeftArrowHit(5)
                .AddInputSequence("Hello ");

            var lowLevelConsole = new LowLevelTestConsole(inputSequence);
            var adapter = new LowLevelToAbstractConsoleAdapter(lowLevelConsole) { Prompt = "(S) " };

            adapter.ReadUntilSequenceIsOver(inputSequence);

            Assert.AreEqual("(S) Hello World", lowLevelConsole.ReadInLineFromTo(0, 0, 14));
            Assert.AreEqual(10, lowLevelConsole.CursorLeft);
        }

        [Test]
        public void ErasesCurrentLineAndResetsCursor()
        {
            var inputSequence = "Hello".ToInputSequence();

            var lowLevelConsole = new LowLevelTestConsole(inputSequence);
            var adapter = new LowLevelToAbstractConsoleAdapter(lowLevelConsole) { Prompt = "(S) " };

            adapter.ReadUntilSequenceIsOver(inputSequence);

            Assert.AreEqual("(S) Hello", lowLevelConsole.ReadInLineFromTo(0, 0, 8));
            Assert.AreEqual(9, lowLevelConsole.CursorLeft);

            adapter.EraseCurrentLine();

            Assert.AreEqual("(S)      ", lowLevelConsole.ReadInLineFromTo(0, 0, 8));
            Assert.AreEqual(4, lowLevelConsole.CursorLeft);
        }
    }
}
