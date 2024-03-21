using Cracker.CommandLine;
using Cracker.CommandLine.App.Commands;

var app = new CommandApp();
app.Add<GuidCommand>("uuid").WithDescription("生成UUID");
app.Add<TestCommand>("test").WithDescription("测试泛型命令");
await app.StartAsync(args);