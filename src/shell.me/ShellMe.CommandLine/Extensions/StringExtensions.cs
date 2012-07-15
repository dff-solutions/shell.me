using System.Linq;

namespace ShellMe.CommandLine.Extensions
{
    public static class StringExtensions
    {
        public static string ToFixedLength(this string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                text = string.Empty;

            var missingCharsCount = length - text.Length >= 0 ? length - text.Length : 0;

            return new string(
                 text.ToCharArray()
                     .Concat(Enumerable.Repeat(' ', missingCharsCount))
                     .Take(length)
                     .ToArray());
        }
    }
}
