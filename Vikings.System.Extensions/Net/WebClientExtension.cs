using System.Threading.Tasks;

namespace System.Net
{
    /// <summary>
    /// WebClient 扩展
    /// </summary>
    public static class WebClientExtension
    {
        /// <summary>
        /// 下载指定的资源
        /// </summary>
        /// <param name="value">要执行的实例</param>
        /// <param name="address">要下载的地址</param>
        /// <param name="timeout">超时，毫秒</param>
        /// <returns>请求的资源</returns>
        public static string DownloadString(this WebClient value, string address, int timeout) =>
            value.Execute(timeout, () => value.DownloadStringTaskAsync(address));

        /// <summary>
        /// 下载指定的资源
        /// </summary>
        /// <param name="value">要执行的实例</param>
        /// <param name="address">要下载的地址</param>
        /// <param name="timeout">超时，毫秒</param>
        /// <returns>请求的资源</returns>
        public static string DownloadString(this WebClient value, Uri address, int timeout) =>
            value.Execute(timeout, () => value.DownloadStringTaskAsync(address));

        /// <summary>
        /// 可指定超时的异步任务
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="value">要执行的实例</param>
        /// <param name="timeout">超时，毫秒</param>
        /// <param name="func">异步方法，比如() =&gt; <see cref="WebClient.DownloadStringTaskAsync(string)"/></param>
        /// <returns>泛型</returns>
        public static T Execute<T>(this WebClient value, int timeout, Func<Task<T>> func)
        {
            return Task.Run(() =>
            {
                var task = func();
                if (!task.Wait(timeout))
                {
                    value.CancelAsync();
                    throw new WebException("Timeout", WebExceptionStatus.Timeout);
                }
                return task.Result;
            }).Result;
        }
    }
}
