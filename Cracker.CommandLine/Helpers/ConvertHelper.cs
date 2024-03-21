namespace Cracker.CommandLine.Helpers
{
    public class ConvertHelper
    {
        public static object? To(object? value, Type type)
        {
            if (type == typeof(TimeSpan))
            {
                if (TimeSpan.TryParseExact(value as string, @"hh\:mm\:ss", null, out var interval))
                {
                    return interval;
                }
            }
            return Convert.ChangeType(value, type);
        }
    }
}