using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ShellMe.CommandLine.Console.LowLevel
{
    public class LowLevelToAbstractConsoleAdapter : AbstractConsole
    {
        private readonly ILowLevelConsole _console;
        private readonly CursorController _cursorController;
        private readonly Subject<ConsoleKeyInfo> _inputs;

        public LowLevelToAbstractConsoleAdapter(ILowLevelConsole console)
        {
            Prompt = "(S) ";
            PromptColor = ConsoleColor.DarkCyan;
            
            _inputs = new Subject<ConsoleKeyInfo>();

            _console = console;
            
            //set the pointer for the current line 
            LineStart = new CursorPosition { CursorLeft = _console.CursorLeft, CursorTop = _console.CursorTop };
            LineEnd = new CursorPosition { CursorLeft = _console.CursorLeft, CursorTop = _console.CursorTop };
            _cursorController = new CursorController(console, LineStart, LineEnd);
        }

        public string Prompt { get; set; }

        public ConsoleColor PromptColor { get; set; }

        public IObservable<ConsoleKeyInfo> KeyStrokes 
        { 
            get { return _inputs.AsObservable(); } 
        }

        public override string ReadLine()
        {
            while (true)
            {
                var readInfo = Read();
                if (readInfo.IsNewLine)
                    return readInfo.Text;
            }
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Read().KeyInfo;
        }

        private void WritePromptIfNeeded()
        {
            if (_cursorController.IsStartOfInput() && _console.CursorLeft == 0)
            {
                var color = _console.ForegroundColor;
                ForegroundColor = PromptColor;
                Write(Prompt);
                _console.ForegroundColor = color;
                LineStart.CursorLeft = _console.CursorLeft;
                LineEnd.CursorLeft = _console.CursorLeft;
            }
        }

        private ReadInfo Read()
        {
            WritePromptIfNeeded();

            var keyInfo = _console.Read();

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (_cursorController.IsStartOfInput())
                    return new ReadInfo { KeyInfo = keyInfo };

                OnBackspaceHit();
            }
            else if(keyInfo.Key == ConsoleKey.Delete)
            {
                if (_cursorController.IsEndOfInput())
                    return new ReadInfo { KeyInfo = keyInfo };

                OnDeleteHit();
            }
            else if(keyInfo.Key == ConsoleKey.Home)
            {
                if(!_cursorController.IsStartOfInput())
                    _cursorController.MoveCursorToHome();
            }
            else if( keyInfo.Key == ConsoleKey.End)
            {
                if(!_cursorController.IsEndOfInput())
                    _cursorController.MoveCursorToEnd();
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                if (!_cursorController.IsStartOfInput())
                    _cursorController.MoveCursorBackward();
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                if (!_cursorController.IsEndOfInput())
                    _cursorController.MoveCursorForward();
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                var inputText = CurrentInput;
                _cursorController.MoveCursorToStartOfNewLine();
                return new ReadInfo {IsNewLine = true, KeyInfo = keyInfo, Text = inputText};
            }
            else if (keyInfo.IsPrintable())
            {
                var textAfterInput = ReadFromCursorToEndOfInput();

                var returnPoint = _cursorController.CreateCursorReturnPoint();

                _console.WriteAtCursorAndMove(keyInfo.KeyChar);
                Write(textAfterInput, false);
                returnPoint();
                _cursorController.MoveCursorForward();
                _cursorController.MoveLineMarkerForward();
            }

            _inputs.OnNext(keyInfo);

            return new ReadInfo {KeyInfo = keyInfo, Text = keyInfo.KeyChar.ToString(CultureInfo.InvariantCulture)};
        }

        private void OnBackspaceHit()
        {
            var textAfterInput = ReadFromCursorToEndOfInput();

            _cursorController.MoveCursorBackward();
            var returnPoint = _cursorController.CreateCursorReturnPoint();
            _console.WriteAtCursorAndMove(' ');
            returnPoint();
            Write(textAfterInput, false);
            returnPoint();
            _cursorController.MoveLineMarkerBackward();
        }

        private void OnDeleteHit()
        {
            var returnPoint = _cursorController.CreateCursorReturnPoint();
            _console.WriteAtCursorAndMove(' ');
            var textAfterInput = ReadFromCursorToEndOfInput();
            returnPoint();
            Write(textAfterInput, false);
            returnPoint();
            _cursorController.MoveLineMarkerBackward();
        }

        public override ConsoleColor ForegroundColor
        {
            get { return _console.ForegroundColor; }
            set { _console.ForegroundColor = value; }
        }

        public string CurrentInput
        {
            get
            {
                var input = ReadFromCoordinatesToEndOfLine(LineStart.CursorLeft, LineStart.CursorTop, false).ToArray();
                var inputText = new string(input);
                return inputText;
            }
        }

        public void Write(string text)
        {
            Write(text.ToCharArray());
        }

        public void Write(IEnumerable<char> keys)
        {
            Write(keys, true);
        }

        private void Write(IEnumerable<char> keys, bool adjustLineMarker)
        {
            foreach (var key in keys)
            {
                _console.WriteAtCursorAndMove(key);
                if(adjustLineMarker)
                    _cursorController.MoveLineMarkerForward();
            }
        }

        public void EraseCurrentLine()
        {
            while (!_cursorController.IsEndOfInput())
            {
                _cursorController.MoveCursorForward();
            }

            while (!_cursorController.IsStartOfInput())
            {
                OnBackspaceHit();
            }
        }

        public override void WriteLine(string text)
        {
            Write(text);
            _cursorController.MoveCursorToStartOfNewLine();
        }

        private IEnumerable<char> ReadFromCursorToEndOfInput()
        {
            return ReadFromCoordinatesToEndOfLine(_console.CursorLeft, _console.CursorTop, true);
        }

        private IEnumerable<char> ReadFromCoordinatesToEndOfLine(int leftStart, int topStart, bool shortCircutEndOfLine)
        {
            var chars = new List<char>();

            //thats the point from where we start to read
            var cursorTop = topStart;
            var initialCursorTop = topStart;
            var cursorLeft = leftStart;

            //if the prompt is the last written character, short circut
            if (shortCircutEndOfLine && LineEnd.CursorLeft == _console.CursorLeft && LineEnd.CursorTop == _console.CursorTop)
                return Enumerable.Empty<char>();

            while (LineEnd.CursorTop >= cursorTop)
            {
                var beginOfLineCursor = cursorTop > initialCursorTop ? 0 : cursorLeft;
                var endOfLineCursor = cursorTop < LineEnd.CursorTop ? _console.MaxColumn : LineEnd.CursorLeft;

                for (int i = beginOfLineCursor; i <= endOfLineCursor; i++)
                {
                    var val = _console.ReadCharacterAt(i, cursorTop);
                    if (val.HasValue)
                        chars.Add(val.Value);
                    else
                        break;
                }
                cursorTop++;
            }
            return chars;
        }

        public CursorPosition LineStart { get; set; }

        public CursorPosition LineEnd { get; set; }

    }

    public class CursorPosition
    {
        public int CursorLeft { get; set; }
        public int CursorTop { get; set; }
    }

    public class CursorController
    {
        private readonly ILowLevelConsole _console;
        private readonly CursorPosition _lineStart;
        private readonly CursorPosition _lineEnd;

        public CursorController(ILowLevelConsole console, CursorPosition lineStart, CursorPosition lineEnd)
        {
            _console = console;
            _lineStart = lineStart;
            _lineEnd = lineEnd;
        }

        public Action CreateCursorReturnPoint()
        {
            var cursorLeft = _console.CursorLeft;
            var cursorTop = _console.CursorTop;
            return () =>
            {
                _console.CursorLeft = cursorLeft;
                _console.CursorTop = cursorTop;
            };
        }

        public void MoveCursorForward()
        {
            if (_console.CursorLeft < _console.MaxColumn)
                _console.CursorLeft++;
            else
            {
                _console.CursorTop++;
                _console.CursorLeft = 0;
            }
        }

        public void MoveCursorBackward()
        {
            if (_console.CursorLeft > 0)
                _console.CursorLeft--;
            else if (_console.CursorTop > 0)
            {
                _console.CursorTop--;
                _console.CursorLeft = _console.MaxColumn;
            }
        }

        public void MoveCursorToStartOfNewLine()
        {
            _console.CursorTop = _lineEnd.CursorTop + 1;
            _console.CursorLeft = 0;

            _lineStart.CursorLeft = _console.CursorLeft;
            _lineStart.CursorTop = _console.CursorTop;
            _lineEnd.CursorLeft = _console.CursorLeft;
            _lineEnd.CursorTop = _console.CursorTop;
        }

        public void MoveCursorToHome()
        {
            _console.CursorTop = _lineStart.CursorTop;
            _console.CursorLeft = _lineStart.CursorLeft;
        }

        public void MoveCursorToEnd()
        {
            _console.CursorTop = _lineEnd.CursorTop;
            _console.CursorLeft = _lineEnd.CursorLeft;
        }

        public bool IsStartOfInput()
        {
            return _console.CursorLeft == _lineStart.CursorLeft && _console.CursorTop == _lineStart.CursorTop;
        }

        public bool IsEndOfInput()
        {
            return _console.CursorLeft == _lineEnd.CursorLeft && _console.CursorTop == _lineEnd.CursorTop;
        }

        public void MoveLineMarkerForward()
        {
            if (_lineEnd.CursorLeft < _console.MaxColumn)
                _lineEnd.CursorLeft++;
            else
            {
                _lineEnd.CursorLeft = 0;
                _lineEnd.CursorTop++;
            }
        }

        public void MoveLineMarkerBackward()
        {
            if (_lineEnd.CursorLeft > 0)
                _lineEnd.CursorLeft--;
            else if (_lineEnd.CursorTop > 0)
            {
                _lineEnd.CursorTop--;
                _lineEnd.CursorLeft = _console.MaxColumn;
            }
        }
    }

    public class ReadInfo
    {
        public bool IsNewLine { get; set; }
        public string Text { get; set; }
        public ConsoleKeyInfo KeyInfo { get; set; }
    }
}
