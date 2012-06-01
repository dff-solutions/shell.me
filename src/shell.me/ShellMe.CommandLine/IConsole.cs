namespace ShellMe.CommandLine
{
    public interface IConsole
    {
        void WriteLine(string line);
        string ReadLine();
    }
}
