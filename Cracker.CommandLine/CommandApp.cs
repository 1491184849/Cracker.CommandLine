using Cracker.CommandLine.Attributes;
using Cracker.CommandLine.Helpers;
using Cracker.CommandLine.Models;
using System.Reflection;

namespace Cracker.CommandLine
{
    public class CommandApp
    {
        private readonly Dictionary<string, CommandConfiguration> _cmds;
        private readonly string[] _helpOptionNames = ["-h", "--help"];
        private readonly string[] _versionOptionNames = ["-v", "--version"];

        public CommandApp()
        {
            _cmds = [];
        }

        /// <summary>
        /// 添加命令
        /// </summary>
        /// <typeparam name="TCommand">命令</typeparam>
        /// <param name="name">命令名称，唯一</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">name存在时抛出</exception>
        public CommandConfiguration Add<TCommand>(string name) where TCommand : CommandBase, new()
        {
            if (_cmds.ContainsKey(name)) throw new ArgumentException($"已存在{name}的命令");
            var conf = new CommandConfiguration(name, new TCommand());
            _cmds.TryAdd(name, conf);
            return conf;
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task StartAsync(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    if (args[0].StartsWith('-'))
                    {
                        if (_helpOptionNames.Contains(args[0]))
                        {
                            Console.WriteLine("选项：");
                            Console.WriteLine("\t{0,-20}\t{1}", string.Join('，', _helpOptionNames), "打印帮助信息");
                            Console.WriteLine("\t{0,-20}\t{1}", string.Join('，', _versionOptionNames), "打印版本信息");
                            Console.WriteLine("命令：");
                            foreach (var item in _cmds)
                            {
                                Console.WriteLine("\t{0,-20}\t{1}", item.Key, item.Value.Description);
                            }
                            Console.WriteLine("\r\n运行 {程序名} [command] --help，获取有关命令的详细信息。");
                        }
                        else if (_versionOptionNames.Contains(args[0]))
                        {
                            Console.WriteLine(Assembly.GetAssembly(typeof(CommandApp))?.GetName().Version);
                        }
                    }
                    else
                    {
                        if (!_cmds.TryGetValue(args[0], out CommandConfiguration? value) || value == null) throw new DirectoryNotFoundException($"命令{args[0]}找不到");
                        var realArgs = args.Skip(1).ToArray();
                        await InvokeAsync(value, realArgs);
                    }
                }
                else
                {
                    Console.WriteLine("欢迎使用自定义命令行！");
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="options"></param>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private static void LoadParams(List<string> arguments, Dictionary<string, string> options, Type type, object instance)
        {
            foreach (var prop in type.GetProperties())
            {
                var optionAttr = prop.GetCustomAttribute<CliOptionAttribute>();
                if (optionAttr != null)
                {
                    if (options.TryGetValue(optionAttr.Name, out var optionValue))
                    {
                        var val = ConvertHelper.To(optionValue, prop.PropertyType);
                        prop.SetValue(instance, val);
                    }
                    else if (optionAttr.DefaultValue != null)
                    {
                        var val = ConvertHelper.To(optionAttr.DefaultValue, prop.PropertyType);
                        prop.SetValue(instance, val);
                    }
                    continue;
                }
                var argAttr = prop.GetCustomAttribute<CliArgumentAttribute>();
                if (argAttr != null)
                {
                    if (arguments.Count >= 1 + argAttr.Position)
                    {
                        if (prop.PropertyType.IsArray)
                        {
                            // 数组参数取值：从位置开始直至末尾
                            string[] strs = [.. arguments[argAttr.Position..]];
                            prop.SetValue(instance, strs);
                        }
                        else
                        {
                            var val = ConvertHelper.To(arguments[argAttr.Position], prop.PropertyType);
                            prop.SetValue(instance, val);
                        }
                    }
                    else if (argAttr.Required)
                    {
                        throw new ArgumentNullException(prop.Name, "参数是必须的");
                    }
                }
            }
        }

        private async Task InvokeAsync(CommandConfiguration value, string[] realArgs)
        {
            var type = value.Instance.GetType();
            var arguments = new List<string>();
            var options = new Dictionary<string, string>();
            for (var i = 0; i < realArgs.Length; i++)
            {
                if (realArgs[i].StartsWith('-'))
                {
                    if (_helpOptionNames.Contains(realArgs[i]) && i == 0)
                    {
                        // 打印帮助信息
                        await Console.Out.WriteLineAsync(value.GetHelp());
                        return;
                    }
                    if (i + 1 < realArgs.Length)
                    {
                        options.TryAdd(realArgs[i], realArgs[i + 1]);
                        break; // 选项只取一个值，后面的值不管
                    }
                }
                else
                {
                    if (value.Children != null && value.Children.TryGetValue(realArgs[i], out CommandConfiguration? sub))
                    {
                        //递归调用子命令：递归到最后一级命令
                        var nextRealArgs = realArgs.Skip(i + 1).Take(realArgs.Length - i - 1);
                        await InvokeAsync(sub, nextRealArgs.ToArray());
                        return;
                    }
                    arguments.Add(realArgs[i]);
                }
            }
            CommandBase instance = value.Instance;
            // 泛型命令
            if (type.BaseType!.IsGenericType)
            {
                var genericType = type.BaseType.GetGenericArguments()[0];
                var genericInstance = Activator.CreateInstance(genericType);
                if (genericInstance != null)
                {
                    LoadParams(arguments, options, genericType, genericInstance);
                    var receiveProp = type.GetProperty(nameof(CommandBase<object>.Receive));
                    receiveProp?.SetValue(instance, genericInstance);
                }
            }
            else
            {
                LoadParams(arguments, options, type, instance);
            }
            await instance.ExecuteAsync(value.Context ?? new CommandContext());
        }
    }
}