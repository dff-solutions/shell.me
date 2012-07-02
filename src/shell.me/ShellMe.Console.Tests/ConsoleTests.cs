using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace ShellMe.Console.Tests
{
    /// <summary>
    /// These tests are end to end and directly launch a ShellMe.Console.exe process.
    /// </summary>
    public class ConsoleTests
    {
        [Test]
        public void IntPropertyInjectionWorks()
        {
            var shellmeConsoleExe = "ShellMe.Console.exe";
            var testFile = "WriteFile.txt";
            Assert.IsFalse(File.Exists(testFile));
            var shellMeConsoleExeFullName = Path.Combine(Environment.CurrentDirectory, shellmeConsoleExe);
            var processStartInfo = new ProcessStartInfo(shellMeConsoleExeFullName, string.Format("writeFile --non-interactive --fileName={0}", testFile));
            processStartInfo.WorkingDirectory = Environment.CurrentDirectory;
            processStartInfo.UseShellExecute = false;
            var process = Process.Start(processStartInfo);
            process.WaitForExit();
            Assert.IsTrue(File.Exists(testFile));
            File.Delete(testFile);
            Assert.IsFalse(File.Exists(testFile));
        }
    }
}
