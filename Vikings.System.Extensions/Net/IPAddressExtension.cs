using System.Linq;
using System.Net.Sockets;

namespace System.Net
{
    /// <summary>
    /// IPAddress 扩展
    /// </summary>
    public static class IPAddressExtension
    {
        /// <summary>
        /// 转为 <see cref="IPAddress"/>
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns><see cref="IPAddress"/> 的新实例</returns>
        public static IPAddress ToIPAddress(this uint value) => new IPAddress(BitConverter.GetBytes(value).Reverse().ToArray());

        /// <summary>
        /// 转为 <see cref="IPAddress"/>
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns><see cref="IPAddress"/> 的新实例</returns>
        public static IPAddress ToIPAddress(this int value) => ((uint)value).ToIPAddress();

        /// <summary>
        /// 转为 <see cref="IPAddress"/>
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns><see cref="IPAddress"/> 的新实例</returns>
        public static IPAddress ToIPAddress(this long value) => ((uint)value).ToIPAddress();

        /// <summary>
        /// 转为 <see cref="uint"/>
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns><see cref="uint"/> 的新实例</returns>
        public static uint ToUInt32(this IPAddress value)
        {
            if (value.AddressFamily != AddressFamily.InterNetwork)
                throw new SocketException((int)SocketError.OperationNotSupported);
            return BitConverter.ToUInt32(value.GetAddressBytes().Reverse().ToArray(), 0);
        }

        /// <summary>
        /// 转为 <see cref="int"/>
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns><see cref="int"/> 的新实例</returns>
        public static int ToInt32(this IPAddress value) => (int)value.ToUInt32();
    }
}
