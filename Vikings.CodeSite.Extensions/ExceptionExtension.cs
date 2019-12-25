using Raize.CodeSiteLogging;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
    /// <summary>
    /// Exception 扩展
    /// </summary>
    public static class ExceptionExtension
    {
        /// <summary>
        /// 输出异常的CodeSite日志
        /// </summary>
        /// <param name="value">要处理的异常</param>
        /// <param name="msg">日志的标识</param>
        /// <returns>始终返回 <see langword="false"/></returns>
        public static bool SendCodeSite(this Exception value, [CallerMemberName] string msg = null)
        {
            string text = value.ToString();
            if (value is WebException we && we.Response is HttpWebResponse hwr)
            {
                if (hwr.ContentType.StartsWith("text/"))
                {
                    Encoding encoding = Encoding.Default;
                    try { encoding = Encoding.GetEncoding(hwr.CharacterSet); }
                    catch (Exception ex) { CodeSite.SendException(hwr.CharacterSet, ex); }
                    try
                    {
                        StreamReader sr = new StreamReader(hwr.GetResponseStream(), encoding);
                        text += Environment.NewLine + sr.ReadToEnd();
                        sr.Close();
                    }
                    catch (Exception ex)
                    {
                        text += Environment.NewLine + ex.ToString();
                    }
                }
            }
            CodeSite.Send(CodeSiteMsgType.Exception, msg ?? value.TargetSite?.ToString(), text);
            return false;
        }
    }
}
