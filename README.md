

# Cracker.CommandLine

* 不依赖第三方包
* 解析命令行参数
* 支持选项、子命令
* 核心使用反射

<!-- PROJECT SHIELDS -->

![NuGet Version][nuget-version-url]
![NuGet Downloads][nuget-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]




###### **使用步骤**

1. 安装.NET SDK8.x
2. 添加nuget包 

`dotnet add package Cracker.CommandLine `

or 

`Install-Package Cracker.CommandLine -Version 1.0.3`

3. 示例Program.cs

```csharp
var app = new CommandApp();
app.Add<ConvertCommand>("convert")
    .Child(() => new CommandConfiguration("ts", new TimeToTimestampCommand()).WithDescription("时间戳转时间"))
    .Child(() => new CommandConfiguration("time", new TimestampToTimeCommand()).WithDescription("时间转时间戳"))
    .WithDescription("格式转换");
await app.StartAsync(args);
```

4. 示例ConvertCommand.cs

```csharp
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
```

### 作者

crackerwork@outlook.com

QQ: 1491184849

MySite: https://crackerwork.cn

 *您也可以在贡献者名单中参看所有参与该项目的开发者。*

### 版权说明

该项目签署了MIT 授权许可，详情请参阅 [LICENSE.txt][license-url]


<!-- links -->
[forks-shield]: https://img.shields.io/github/forks/1491184849/Cracker.CommandLine.svg?style=flat-square
[forks-url]: https://github.com/1491184849/Cracker.CommandLine/network/members
[stars-shield]: https://img.shields.io/github/stars/1491184849/Cracker.CommandLine.svg?style=flat-square
[stars-url]: https://github.com/1491184849/Cracker.CommandLine/stargazers
[issues-shield]: https://img.shields.io/github/issues/1491184849/Cracker.CommandLine.svg?style=flat-square
[issues-url]: https://img.shields.io/github/issues/1491184849/Cracker.CommandLine.svg
[license-shield]: https://img.shields.io/github/license/1491184849/Cracker.CommandLine.svg?style=flat-square
[license-url]: https://github.com/1491184849/Cracker.CommandLine/blob/master/LICENSE.txt
[nuget-url]: https://img.shields.io/nuget/dt/Cracker.CommandLine
[nuget-version-url]: https://img.shields.io/nuget/v/Cracker.CommandLine




