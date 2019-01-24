using System;
using System.Net;

namespace Vikings.Net
{
    public class WebClient: System.Net.WebClient
    {
        /// <summary>
        /// 获取或设置请求超时之前的时间长度（以毫秒为单位）。
        /// </summary>
        public virtual int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address);
            if (Timeout != 0) webRequest.Timeout = Timeout;
            return webRequest;
        }
    }
}
