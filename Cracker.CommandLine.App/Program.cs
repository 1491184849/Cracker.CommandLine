using Cracker.CommandLine;
using Cracker.CommandLine.App.Commands;

var app = new CommandApp();
app.Add<GuidCommand>("uuid").WithDescription("生成UUID");
await app.StartAsync(args);