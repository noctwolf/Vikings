using System;
using System.Net;

namespace Vikings.Net
{
    /// <summary>
    /// 扩展<see cref="System.Net.WebClient"/>
    /// </summary>
    public class WebClient: System.Net.WebClient
    {
        /// <summary>
        /// 获取或设置请求超时之前的时间长度（以毫秒为单位）。
        /// </summary>
        public virtual int Timeout { get; set; }

        /// <summary>
        /// 重载<see cref="System.Net.WebClient.GetWebRequest"/>，设置超时<see cref="WebRequest.Timeout"/>
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address);
            if (Timeout != 0) webRequest.Timeout = Timeout;
            return webRequest;
        }
    }
}
