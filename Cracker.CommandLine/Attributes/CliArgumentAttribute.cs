namespace Cracker.CommandLine.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CliArgumentAttribute(int position, string description, bool required = false) : Attribute
    {
        /// <summary>
        /// 参数位置，使用索引值
        /// </summary>
        public int Position => position;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description => description;

        /// <summary>
        /// 是否必须
        /// </summary>
        public bool Required => required;
    }
}