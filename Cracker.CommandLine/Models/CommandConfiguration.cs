using Cracker.CommandLine.Attributes;
using System.Reflection;
using System.Text;

namespace Cracker.CommandLine.Models
{
    public class CommandConfiguration
    {
        public CommandConfiguration(string name, CommandBase instance)
        {
            Name = name;
            Instance = instance;
            HelpInformation = GetCommandHelpInformation();
        }

        public CommandConfiguration WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public CommandConfiguration WithData(object data)
        {
            Context = new CommandContext { Data = data };
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
        public string Description { get; private set; }

        /// <summary>
        /// 上下文
        /// </summary>
        public CommandContext Context { get; private set; }

        /// <summary>
        /// 帮助信息
        /// </summary>
        public string HelpInformation { get; private set; }

        private string GetCommandHelpInformation()
        {
            var type = Instance.GetType();
            if (type.BaseType != null && type.BaseType.IsGenericType)
            {
                type = type.BaseType!.GetGenericArguments()[0]!;
            }
            var options = new Dictionary<string, string>();
            var arguments = new Dictionary<int, string>();
            var sb = new StringBuilder();
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
                    sb.AppendFormat("\t[{0}]\t{1}\r\n", item.Key + 1, item.Value);
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
    }
}