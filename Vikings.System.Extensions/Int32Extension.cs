using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
    /// <summary>
    /// int 扩展
    /// </summary>
    public static class Int32Extension
    {
        /// <summary>
        /// 把长度尽量均匀切分
        /// </summary>
        /// <param name="total">要切分的长度</param>
        /// <param name="count">要切分的份数</param>
        /// <returns>每一段长度的可枚举集合</returns>
        public static IEnumerable<int> Split(this int total, int count)
        {
            if (total <= 0) throw new ArgumentException("参数值必须大于零", nameof(total));
            if (count <= 0) throw new ArgumentException("参数值必须大于零", nameof(count));
            double split = total * 1d / count;
            for (int i = 0; i < count; i++)
                yield return Convert.ToInt32(split * (i + 1)) - Convert.ToInt32(split * i);
        }

        /// <summary>
        /// 将数值转换为字符串，该字符串表示以字节，KB，MB或GB表示的大小值表示的数字，具体取决于大小。
        /// </summary>
        /// <param name="length">要转换的数值</param>
        /// <returns>数值字符串</returns>
        public static string ToByteSize(this int length) => ((long)length).ToByteSize();

        /// <summary>
        /// 将数值转换为字符串，该字符串表示以字节，KB，MB或GB表示的大小值表示的数字，具体取决于大小。
        /// </summary>
        /// <param name="length">要转换的数值</param>
        /// <returns>数值字符串</returns>
        public static string ToByteSize(this long length)
        {
            StringBuilder sb = new StringBuilder(64);
            StrFormatByteSizeW(length, sb, sb.Capacity);
            return sb.ToString();
        }

        [DllImport("shlwapi", CharSet = CharSet.Unicode)]
        static extern IntPtr StrFormatByteSizeW(long qdw, StringBuilder pszBuf, int cchBuf);
    }
}
