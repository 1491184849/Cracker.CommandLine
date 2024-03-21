# 自定义命令行

* 纯手写代码实现
* 接收命令行参数，解析、赋值
* 核心使用反射

## 快速开始

**nuget安装**

`dotnet add package Cracker.CommandLine `

**Main方法**

```csharp
var app = new CommandApp();
app.Add<GuidCommand>("uuid").WithDescription("生成UUID");
app.Add<TestCommand>("test").WithDescription("测试泛型命令");
await app.StartAsync(args);
```

## GuidCommand.cs

```csharp
public class GuidCommand : CommandBase
{
    public override Task ExecuteAsync(CommandContext context)
    {
        Console.WriteLine(Guid.NewGuid());
        return Task.CompletedTask;
    }
}
```

## TestCommand.cs

```csharp
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
```

## 特性

* CliArgumentAttribute: 接收命令参数
* CliOptionAttribute: 接收选项参数