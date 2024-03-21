using System.Diagnostics.CodeAnalysis;

namespace Cracker.CommandLine.Models
{
    public abstract class CommandBase
    {
        public abstract Task ExecuteAsync(CommandContext context);
    }

    public abstract class CommandBase<T>: CommandBase where T : class, new()
    {
        [NotNull]
        public T? Receive { get; set; }
    }
}