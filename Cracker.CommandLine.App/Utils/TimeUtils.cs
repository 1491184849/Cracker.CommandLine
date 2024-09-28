namespace Cracker.CommandLine.App.Utils
{
    public class TimeUtils
    {
        private static DateTime _unixTime = new(1970, 1, 1, 0, 0, 0);

        /// <summary>
        /// 时间转时间戳
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="isSecond">true秒|false毫秒</param>
        /// <returns></returns>
        public static long DateTimeToTimestamp(DateTime time, bool isSecond)
        {
            if (isSecond)
            {
                return (long)time.ToUniversalTime().Subtract(_unixTime).TotalSeconds;
            }
            return (long)time.ToUniversalTime().Subtract(_unixTime).TotalMilliseconds;
        }

        /// <summary>
        /// 时间戳转时间
        /// </summary>
        /// <param name="ts">10位秒时间戳，13位毫秒时间戳</param>
        /// <returns></returns>
        public static DateTime TimestampToDateTime(long ts)
        {
            int len = ts.ToString().Length;
            if (len == 10) return _unixTime.AddSeconds(ts).ToLocalTime();
            else if (len == 13) return _unixTime.AddMilliseconds(ts).ToLocalTime();
            throw new NotSupportedException("不支持的时间戳");
        }
    }
}