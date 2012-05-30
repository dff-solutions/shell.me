namespace ShellMe.Console.Configuration
{
    public interface ICommandBundle
    {
        string CommandName { get; }
        BaseCommandConfiguration CreateConfiguration(ArgumentsProvider argumentsProvider);
        IProcessor CreateProcessor(BaseCommandConfiguration commandConfiguration);
    }
}
