using Cracker.CommandLine.Attributes;
using Cracker.CommandLine.Helpers;
using Cracker.CommandLine.Models;
using System.Reflection;

namespace Cracker.CommandLine
{
    public class CommandApp
    {
        private readonly IDictionary<string, CommandConfiguration> _cmds;
        private readonly string[] _helpOptionNames = ["-h", "--help"];
        private readonly string[] _versionOptionNames = ["-v", "--version"];

        public CommandApp()
        {
            _cmds = new Dictionary<string, CommandConfiguration>();
        }

        /// <summary>
        /// 添加命令
        /// </summary>
        /// <typeparam name="TCommand">命令</typeparam>
        /// <param name="name">命令名称，唯一</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CommandConfiguration Add<TCommand>(string name) where TCommand : CommandBase, new()
        {
            if (_cmds.ContainsKey(name)) throw new Exception($"已存在{name}的命令");
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
                    if (args[0].Contains('-'))
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
                        }
                        else if (_versionOptionNames.Contains(args[0]))
                        {
                            Console.WriteLine("v1.0.0");
                        }
                    }
                    else
                    {
                        if (!_cmds.TryGetValue(args[0], out CommandConfiguration? value) || value == null) throw new DirectoryNotFoundException($"命令{args[0]}找不到");
                        var type = value.Instance.GetType();
                        var skipArgs = args.Skip(1).ToArray();
                        var arguments = new List<string>();
                        var options = new Dictionary<string, string>();
                        for (var i = 0; i < skipArgs.Length; i++)
                        {
                            if (skipArgs[i].StartsWith('-'))
                            {
                                if (_helpOptionNames.Contains(skipArgs[i]))
                                {
                                    // 打印帮助信息
                                    await Console.Out.WriteLineAsync(value.HelpInformation);
                                    return;
                                }
                                if (i + 1 < skipArgs.Length)
                                {
                                    options.TryAdd(skipArgs[i], skipArgs[i + 1]);
                                    break; // 选项只取一个值，后面的值不算参数不管
                                }
                            }
                            else
                            {
                                arguments.Add(skipArgs[i]);
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
                if (optionAttr != null && options.TryGetValue(optionAttr.Name, out var optionValue))
                {
                    var val = ConvertHelper.To(optionValue, prop.PropertyType);
                    prop.SetValue(instance, val);
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
    }
}