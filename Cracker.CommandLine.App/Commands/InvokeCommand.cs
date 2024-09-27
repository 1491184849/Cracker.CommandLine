using Cracker.CommandLine.Models;

namespace Cracker.CommandLine.App.Commands
{
    public class InvokeCommand : CommandBase
    {
        public override Task ExecuteAsync(CommandContext context)
        {

            return Task.CompletedTask;
        }
    }
}