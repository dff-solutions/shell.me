namespace ShellMe.CommandLine.CommandHandling
{
    public interface ITraceableCommand
    {
        string WriteFile { get; set; }
        bool WriteEventLog { get; set; }
    }
}
