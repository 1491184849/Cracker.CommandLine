namespace Cracker.CommandLine.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CliOptionAttribute(string name, string description, object? defaultValue = default) : Attribute
    {
        /// <summary>
        /// 选项名，例：-o
        /// </summary>
        public string Name => name;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description => description;

        /// <summary>
        /// 默认值
        /// </summary>
        public object? DefaultValue => defaultValue;
    }
}