using Cracker.CommandLine;
using Cracker.CommandLine.App.Commands;
using Cracker.CommandLine.Models;

var app = new CommandApp();
app.Add<ConvertCommand>("convert")
    .Child(() => new CommandConfiguration("ts", new TimeToTimestampCommand()).WithDescription("时间戳转时间"))
    .Child(() => new CommandConfiguration("time", new TimestampToTimeCommand()).WithDescription("时间转时间戳"))
    .WithDescription("格式转换");
await app.StartAsync(args);