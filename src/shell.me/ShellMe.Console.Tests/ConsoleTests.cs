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
            
            var testFile = "WriteFile.txt";
            Assert.IsFalse(File.Exists(testFile));
            var process = InvokeShellMe(string.Format("writeFile --non-interactive --fileName={0}", testFile));
            process.WaitForExit();
            Assert.IsTrue(File.Exists(testFile));
            File.Delete(testFile);
            Assert.IsFalse(File.Exists(testFile));
        }

        private Process InvokeShellMe(string fullCommand)
        {
            var shellMeConsoleExe = "ShellMe.Console.exe";
            var shellMeConsoleExeFullName = Path.Combine(Environment.CurrentDirectory, shellMeConsoleExe);
            var processStartInfo = new ProcessStartInfo(shellMeConsoleExeFullName, fullCommand);
            processStartInfo.WorkingDirectory = Environment.CurrentDirectory;
            processStartInfo.UseShellExecute = false;
            return Process.Start(processStartInfo);
        }
    }
}
