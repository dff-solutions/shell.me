using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ShellMe.CommandLine.Console.LowLevel;

namespace ShellMe.Testing
{
    public static class UserInputStreamHelper
    {
        public static IEnumerable<ConsoleKeyInfo> ToInputSequence(this string text)
        {
            return text.Select(c =>
                                   {
                                       ConsoleKey consoleKey;
                                       if (Enum.TryParse(c.ToString(CultureInfo.InvariantCulture), true, out consoleKey))
                                       {
                                           return new ConsoleKeyInfo(c, consoleKey, false, false, false);
                                       }
                                       if (c == ' ')
                                           return new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false);
                                       if (c == '.')
                                           return new ConsoleKeyInfo('.', ConsoleKey.OemPeriod, false, false, false);
                                       if (c == ',')
                                           return new ConsoleKeyInfo(',', ConsoleKey.OemComma, false, false, false);
                                       if (c == '-')
                                           return new ConsoleKeyInfo('-', ConsoleKey.OemMinus, false, false, false);
                                       if (c == '+')
                                           return new ConsoleKeyInfo('+', ConsoleKey.OemPlus, false, false, false);
                                       if (c == '!')
                                           return new ConsoleKeyInfo('!', ConsoleKey.D1, true, false, false);
                                       if (c == '=')
                                           return new ConsoleKeyInfo('=', ConsoleKey.D0, true, false, false);
                                       if (c == '[')
                                           return new ConsoleKeyInfo('[', ConsoleKey.D8, false, true, false);
                                       if (c == ']')
                                           return new ConsoleKeyInfo(']', ConsoleKey.D9, false, true, false);

                                       return (ConsoleKeyInfo?) null;
                                   })
                .Where(info => info.HasValue)
                .Select(info => info.GetValueOrDefault());
        }

        public static IEnumerable<ConsoleKeyInfo> AddInputSequence(this IEnumerable<ConsoleKeyInfo> sequence, string text)
        {
            return sequence.Concat(text.ToInputSequence());
        }

        public static IEnumerable<ConsoleKeyInfo> AddEnterHit(this IEnumerable<ConsoleKeyInfo> sequence)
        {
            return sequence.Concat(Enumerable.Repeat(new ConsoleKeyInfo(default(char), ConsoleKey.Enter, false, false, false), 1));
        }

        public static IEnumerable<ConsoleKeyInfo> AddBackspaceHit(this IEnumerable<ConsoleKeyInfo> sequence)
        {
            return sequence.Concat(Enumerable.Repeat(new ConsoleKeyInfo(default(char), ConsoleKey.Backspace, false, false, false), 1));
        }

        public static IEnumerable<ConsoleKeyInfo> AddBackspaceHit(this IEnumerable<ConsoleKeyInfo> sequence, int times)
        {
            return sequence.Concat(Enumerable.Repeat(new ConsoleKeyInfo(default(char), ConsoleKey.Backspace, false, false, false), times));
        }

        public static IEnumerable<ConsoleKeyInfo> AddLeftArrowHit(this IEnumerable<ConsoleKeyInfo> sequence)
        {
            return sequence.Concat(Enumerable.Repeat(new ConsoleKeyInfo(default(char), ConsoleKey.LeftArrow, false, false, false), 1));
        }

        public static IEnumerable<ConsoleKeyInfo> AddLeftArrowHit(this IEnumerable<ConsoleKeyInfo> sequence, int times)
        {
            return sequence.Concat(Enumerable.Repeat(new ConsoleKeyInfo(default(char), ConsoleKey.LeftArrow, false, false, false), times));
        }

        public static IEnumerable<ConsoleKeyInfo> AddRightArrowHit(this IEnumerable<ConsoleKeyInfo> sequence)
        {
            return sequence.Concat(Enumerable.Repeat(new ConsoleKeyInfo(default(char), ConsoleKey.RightArrow, false, false, false), 1));
        }

        public static IEnumerable<ConsoleKeyInfo> AddRightArrowHit(this IEnumerable<ConsoleKeyInfo> sequence, int times)
        {
            return sequence.Concat(Enumerable.Repeat(new ConsoleKeyInfo(default(char), ConsoleKey.RightArrow, false, false, false), times));
        }

        public static void ReadUntilSequenceIsOver(this LowLevelToAbstractConsoleAdapter adapter, IEnumerable<ConsoleKeyInfo> inputSequence)
        {
            for (int i = 0; i < inputSequence.Count(); i++)
            {
                adapter.ReadKey();
            }
        }
    }

    public class LowLevelTestConsole : ILowLevelConsole
    {
        private readonly IEnumerator<ConsoleKeyInfo> _keySequenceEnumerator;
        private readonly List<List<char?>> _buffer;

        public LowLevelTestConsole():this(Enumerable.Empty<ConsoleKeyInfo>())
        {
        }

        public LowLevelTestConsole(IEnumerable<ConsoleKeyInfo> keySequence)
        {
            _keySequenceEnumerator = keySequence.GetEnumerator();
            _buffer = new List<List<char?>>();
            MaxColumn = 79;
            AddLineToBuffer();
        }

        private void AddLineToBuffer()
        {
            _buffer.Add(Enumerable.Repeat((char?)' ',MaxColumn + 1).ToList());
        }

        private void AddLinesToBuffer(int lines)
        {
            for (int i = 0; i < lines; i++)
            {
                AddLineToBuffer();
            }
        }

        public int BufferLines
        {
            get { return _buffer.Count; }
        }

        public int MaxColumn { get; private set; }

        public ConsoleColor ForegroundColor { get; set; }

        public int CursorLeft { get; set; }

        public int CursorTop { get; set; }

        public char? ValueAtCursor
        {
            get { return _buffer[CursorTop][CursorLeft]; }
        }

        public void WriteAtCursorAndMove(char key)
        {
            //There is no other way to calculate when to insert a line.
            //That's because if the higher level code writes Console.WriteLine("")
            //This method will never get called. This results in an offset where we need
            //to insert several missing lines at once

            var currentMaxCursor = _buffer.Count - 1;
            var missingLines = CursorTop - currentMaxCursor;
            if (missingLines > 0)
                AddLinesToBuffer(missingLines);

            _buffer[CursorTop][CursorLeft] = key;

            if (CursorLeft == (MaxColumn - 1))
            {
                CursorLeft = 0;
                CursorTop++;
            }
            else
            {
                CursorLeft++;
            }
        }

        public ConsoleKeyInfo Read()
        {
            _keySequenceEnumerator.MoveNext();
            var value = _keySequenceEnumerator.Current;
            return value;
        }

        public char? ReadCharacterAt(int x, int y)
        {
            return _buffer[y][x]; 
        }
    }
}
