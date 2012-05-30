

namespace ShellMe.Console
{
    public class NativeConsoleWrapper : IConsole
    {
        public void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }

        public string ReadLine()
        {
            return System.Console.ReadLine();
        }
    }
}
