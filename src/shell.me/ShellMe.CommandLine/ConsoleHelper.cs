using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShellMe.CommandLine.Console;

namespace ShellMe.CommandLine
{
    static class ConsoleHelper
    {
        public static void WriteLineInRed(AbstractConsole console, string text)
        {
            WriteAndResetColor(ConsoleColor.Red, console, text);
        }

        public static void WriteLineInGreen(AbstractConsole console, string text)
        {
            WriteAndResetColor(ConsoleColor.Green, console, text);
        }

        public static void WriteAndResetColor(ConsoleColor color, AbstractConsole console, string text)
        {
            var resetPoint = CreateColorResetPoint(console);
            console.ForegroundColor = color;
            console.WriteLine(text);
            resetPoint();
        }

        public static Action CreateColorResetPoint(AbstractConsole console)
        {
            var currentColor = console.ForegroundColor;
            return () => console.ForegroundColor = currentColor;
        }
    }
}
