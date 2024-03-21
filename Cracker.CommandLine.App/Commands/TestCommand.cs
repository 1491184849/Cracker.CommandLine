using Cracker.CommandLine.Attributes;
using Cracker.CommandLine.Models;

namespace Cracker.CommandLine.App.Commands
{
    public class TestReceive
    {
        [CliArgument(0, "参数1")]
        public string? Val1 { get; set; }

        [CliArgument(1, "参数2")]
        public string? Val2 { get; set; }

        [CliOption("-o", "测试")]
        public string? Option1 { get; set; }
    }

    public class TestCommand : CommandBase<TestReceive>
    {
        public override Task ExecuteAsync(CommandContext context)
        {
            Console.WriteLine("参数1：" + Receive.Val1);
            Console.WriteLine("参数2：" + Receive.Val2);
            Console.WriteLine("测试选项1：" + Receive.Option1);
            return Task.CompletedTask;
        }
    }
}