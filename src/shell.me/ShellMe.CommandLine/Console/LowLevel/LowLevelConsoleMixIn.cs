using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine.Console.LowLevel
{
    public static class LowLevelConsoleMixIn
    {
        public static string ReadFromTo(this ILowLevelConsole console, int xStart, int yStart, int xEnd, int yEnd)
        {
            throw new NotImplementedException();
        }

        public static string ReadInLineFromTo(this ILowLevelConsole console, int lineIndex, int startIndex, int endIndex)
        {
            var chars = new List<char>();
            for (int i = startIndex; i <= endIndex ; i++)
            {
                var currentChar = console.ReadCharacterAt(i, lineIndex);
                if (currentChar.HasValue)
                    chars.Add(currentChar.Value);
            }
            return new string(chars.ToArray());
        }
    }
}
