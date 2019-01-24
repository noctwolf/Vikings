using System.Threading.Tasks;

namespace System.Net
{
    public static class WebClientExtension
    {
        public static string DownloadString(this WebClient value, string address, int timeout)
        {
            return DownloadString(value, timeout, () => value.DownloadStringTaskAsync(address));
        }

        public static string DownloadString(this WebClient value, Uri address, int timeout)
        {
            return DownloadString(value, timeout, () => value.DownloadStringTaskAsync(address));
        }

        private static string DownloadString(WebClient value, int timeout, Func<Task<string>> func)
        {
            return Task.Run(() =>
            {
                var t = func();
                if (!t.Wait(timeout))
                {
                    value.CancelAsync();
                    throw new WebException("Timeout", WebExceptionStatus.Timeout);
                }
                return t.Result;
            }).Result;
        }
    }
}
