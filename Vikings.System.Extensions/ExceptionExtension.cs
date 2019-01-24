using Raize.CodeSiteLogging;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
    public static class ExceptionExtension
    {
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
                    StreamReader sr = new StreamReader(hwr.GetResponseStream(), encoding);
                    text += Environment.NewLine + sr.ReadToEnd();
                    sr.Close();
                }
            }
            CodeSite.Send(CodeSiteMsgType.Exception, msg ?? value.TargetSite?.ToString(), text);
            return false;
        }
    }
}
