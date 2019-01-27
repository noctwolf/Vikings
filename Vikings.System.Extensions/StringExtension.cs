using System.Linq;

namespace System
{
    /// <summary>
    /// String 扩展
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 从此实例检索子字符串
        /// </summary>
        /// <param name="value">要检索的实例</param>
        /// <param name="start">开始标志</param>
        /// <param name="end">结束标志</param>
        /// <returns>如果没找到则返回 <see cref="string.Empty"/></returns>
        public static string Substring(this string value, string start, string end)
        {
            int iStart = value.IndexOf(start);
            if (iStart > -1)
            {
                iStart = iStart + start.Length;
                int iEnd = value.IndexOf(end, iStart);
                if (iEnd > -1) return value.Substring(iStart, iEnd - iStart);
            }
            return string.Empty;
        }

        /// <summary>
        /// 从此实例返回有效文件名
        /// </summary>
        /// <param name="value">要处理的实例</param>
        /// <returns>去掉无效文件名字符后的字符串</returns>
        public static string ToValidFileName(this string value) => 
            string.Join("", value?.ToCharArray().Where(c => !IO.Path.GetInvalidFileNameChars().Contains(c)));
    }
}