using Raize.CodeSiteLogging;

namespace System
{
    public static class ExceptionExtensions
    {
        public static void SendCodeSite(this Exception value, string msg = null)
        {
            if (msg == null) msg = value.TargetSite.Name;
            CodeSite.Send(CodeSiteMsgType.Exception, msg, value.ToString());
        }
    }
}
