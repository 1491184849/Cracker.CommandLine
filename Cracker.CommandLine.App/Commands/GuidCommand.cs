using Cracker.CommandLine.Models;

namespace Cracker.CommandLine.App.Commands
{
    public class GuidCommand : CommandBase
    {
        public override Task ExecuteAsync(CommandContext context)
        {
            Console.WriteLine(Guid.NewGuid());
            return Task.CompletedTask;
        }
    }
}