using System;
using System.Diagnostics;

namespace ShellMe.CommandLine
{
    /// <summary>
    /// Replacement for the built in TextWriterTraceListener. Puts a timestamp in front of any line
    /// </summary>
    public class TimestampedTextWriterTraceListener : TextWriterTraceListener
    {
        public TimestampedTextWriterTraceListener(string fileName) :base(fileName)
        {
        }

        public override void Write(string message)
        {
            base.Write(ToDatedString(message));
        }

        private string ToDatedString(string message)
        {
            return string.Format("{0}|{1}", DateTime.Now.ToString("d.M.yyyy HH:mm:ss"), message);
        }
    }
}
