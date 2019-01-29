using System.Diagnostics;
using System.Reflection;

namespace System.Windows
{
    /// <summary>
    /// Application 扩展
    /// </summary>
    public static class ApplicationExtension
    {
        /// <summary>
        /// 获取当前应用程序的可执行文件的路径，包括可执行文件的名称。
        /// </summary>
        /// <param name="value">忽略</param>
        /// <returns>当前应用程序的可执行文件的路径，包括可执行文件的名称</returns>
        public static string ExecutablePath(this Application value)
        {
            Uri uri = new Uri(Assembly.GetEntryAssembly().CodeBase);
            if (uri.IsFile) return uri.LocalPath + Uri.UnescapeDataString(uri.Fragment);
            else return uri.ToString();
        }

        /// <summary>
        /// 关闭应用程序并立即启动一个新实例
        /// </summary>
        /// <param name="value">忽略</param>
        public static void Restart(this Application value)
        {
            string cmdLine = Environment.CommandLine;
            string cmdLineArgs0 = Environment.GetCommandLineArgs()[0];
            int i = cmdLine.IndexOf(' ', cmdLine.IndexOf(cmdLineArgs0) + cmdLineArgs0.Length);
            cmdLine = cmdLine.Remove(0, i + 1);

            ProcessStartInfo startInfo = Process.GetCurrentProcess().StartInfo;
            startInfo.FileName = value.ExecutablePath();
            startInfo.Arguments = cmdLine;
            value.Shutdown();
            Process.Start(startInfo);
        }
    }
}
