using Cracker.CommandLine.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Cracker.CommandLine.Models
{
    public class CommandConfiguration
    {
        /// <summary>
        /// 加载命令时，自动生成的帮助信息
        /// </summary>
        private string? _helpInformation;

        public CommandConfiguration(string name, CommandBase instance)
        {
            if (name.StartsWith('-'))
            {
                throw new Exception("命令不允许'-'开头");
            }
            Name = name;
            Instance = instance;
            _helpInformation = GenerateCommandHelpInformation();
        }

        /// <summary>
        /// 命令描述
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public CommandConfiguration WithDescription(string description)
        {
            Description = description;
            _helpInformation = GenerateCommandHelpInformation();
            return this;
        }

        /// <summary>
        /// 初始数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CommandConfiguration WithData(object data)
        {
            Context = new CommandContext { Data = data };
            return this;
        }

        /// <summary>
        /// 增加子命令
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        /// <exception cref="Exception">子命令不能超过3个</exception>
        public CommandConfiguration Child(Func<CommandConfiguration> func)
        {
            Children ??= [];
            if (Children.Count >= 3)
            {
                throw new Exception("子命令不能超过3个");
            }
            var subCommandConfiguration = func.Invoke();
            Children.TryAdd(subCommandConfiguration.Name, subCommandConfiguration);
            return this;
        }

        /// <summary>
        /// 命令名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 命令实例
        /// </summary>
        public CommandBase Instance { get; private set; }

        /// <summary>
        /// 命令描述
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// 上下文
        /// </summary>
        [NotNull]
        public CommandContext? Context { get; private set; }

        /// <summary>
        /// 子命令
        /// </summary>
        public Dictionary<string, CommandConfiguration>? Children { get; private set; }

        private string GenerateCommandHelpInformation()
        {
            var type = Instance.GetType();
            if (type.BaseType != null && type.BaseType.IsGenericType)
            {
                type = type.BaseType!.GetGenericArguments()[0]!;
            }
            var options = new Dictionary<string, string>();
            var arguments = new Dictionary<int, string>();
            var sb = new StringBuilder();
            sb.AppendFormat("描述：\t{0,-20}\r\n", Description);
            foreach (var prop in type.GetProperties())
            {
                var optionAttr = prop.GetCustomAttribute<CliOptionAttribute>();
                if (optionAttr != null) options.TryAdd(optionAttr.Name, optionAttr.Description);
                var argAttr = prop.GetCustomAttribute<CliArgumentAttribute>();
                if (argAttr != null)
                {
                    if (arguments.ContainsKey(argAttr.Position)) throw new Exception("参数位置不能重复");
                    arguments.TryAdd(argAttr.Position, argAttr.Description);
                }
                var arrayType = typeof(string[]);
                if (prop.PropertyType.IsArray && prop.PropertyType != arrayType)
                {
                    throw new ArgumentException("数组参数只支持" + arrayType);
                }
            }
            if (arguments.Count > 0)
            {
                sb.AppendLine("参数：");
                foreach (var item in arguments.OrderBy(x => x.Key))
                {
                    sb.AppendFormat("\t{0,-20}\t{1}\r\n", string.Concat("位置", item.Key + 1), item.Value);
                }
            }
            if (options.Count > 0)
            {
                sb.AppendLine("选项：");
                foreach (var item in options)
                {
                    sb.AppendFormat("\t{0,-20}\t{1}\r\n", item.Key, item.Value);
                }
            }
            return sb.ToString();
        }

        public string? GetHelp()
        {
            return _helpInformation;
        }
    }
}