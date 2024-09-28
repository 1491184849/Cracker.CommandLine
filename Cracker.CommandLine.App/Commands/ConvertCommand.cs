using Cracker.CommandLine.App.Utils;
using Cracker.CommandLine.Attributes;
using Cracker.CommandLine.Models;

namespace Cracker.CommandLine.App.Commands
{
    public class ConvertCommand : CommandBase
    {
        public override Task ExecuteAsync(CommandContext context)
        {
            return Task.CompletedTask;
        }
    }

    public class TimeParameter
    {
        [CliArgument(0, "要转换的时间|时间戳", true)]
        public string? Value { get; set; }

        [CliOption("-f", "指定时间戳的格式，可选值：<s|ms>，s：秒，ms:毫秒", "s")]
        public string? Format { get; set; }
    }

    public class TimeToTimestampCommand : CommandBase<TimeParameter>
    {
        public override Task ExecuteAsync(CommandContext context)
        {
            if (DateTime.TryParse(Receive.Value, out var time))
            {
                if (Receive.Format == "s")
                {
                    Console.WriteLine(TimeUtils.DateTimeToTimestamp(time, true));
                }
                else
                {
                    Console.WriteLine(TimeUtils.DateTimeToTimestamp(time, false));
                }
            }
            else
            {
                Console.WriteLine("时间格式错误");
            }
            return Task.CompletedTask;
        }
    }

    public class TimestampToTimeCommand : CommandBase<TimeParameter>
    {
        public override Task ExecuteAsync(CommandContext context)
        {
            if (long.TryParse(Receive.Value, out var ts))
            {
                Console.WriteLine(TimeUtils.TimestampToDateTime(ts).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                Console.WriteLine("时间戳格式错误");
            }
            return Task.CompletedTask;
        }
    }
}