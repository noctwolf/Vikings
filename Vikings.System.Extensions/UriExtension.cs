using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    /// Uri 扩展
    /// </summary>
    public static class UriExtension
    {
        /// <summary>
        /// 从此字符串中删除双向控制字符
        /// </summary>
        /// <param name="s">要处理的字符串</param>
        /// <returns>删除双向控制字符后的字符串</returns>
        public static string StripBidiControlCharacter(this string s) => string.Concat(s.ToCharArray().Where(c => !c.IsBidiControlCharacter()));

        /// <summary>
        /// 是否属于双向控制字符
        /// </summary>
        /// <param name="c">要判断的字符</param>
        /// <returns>如果是双向控制字符，返回<see langword="true"/>，否则返回<see langword="false"/></returns>
        public static bool IsBidiControlCharacter(this char c) => (bool)typeof(Uri)
            .GetMethod("IsBidiControlCharacter", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { c });
    }
}
