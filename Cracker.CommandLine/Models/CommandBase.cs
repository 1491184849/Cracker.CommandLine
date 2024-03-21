namespace Cracker.CommandLine.Models
{
    public abstract class CommandBase
    {
        public abstract Task ExecuteAsync(CommandContext context);
    }
}