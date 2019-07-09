using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Vikings.Collections.Generic
{
    /// <summary>
    /// 比较两个Unicode字符串。字符串中的数字被视为数字内容而不是文本。此测试不区分大小写。
    /// </summary>
    public class StringLogicalComparer : Comparer<string>
    {
        [DllImport("Shlwapi", SetLastError = false, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string psz1, string psz2);

        /// <summary>
        /// 内部调用 StrCmpLogicalW 比较
        /// </summary>
        /// <param name="x">要比较的第一个对象</param>
        /// <param name="y">要比较的第二个对象</param>
        /// <returns></returns>
        public override int Compare(string x, string y) => StrCmpLogicalW(x, y);
    }
}
