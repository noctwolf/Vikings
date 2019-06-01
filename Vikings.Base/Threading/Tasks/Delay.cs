using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Vikings.Threading.Tasks
{
    /// <summary>
    /// 延迟执行
    /// </summary>
    public static class Delay
    {
        private class Caller
        {
            protected internal CancellationTokenSource CancellationTokenSource;
            protected internal DateTime LastTime;
        }

        [ThreadStatic]
        private static Dictionary<string, Caller> callerDict;

        private static void ThreadInit() => callerDict = callerDict ?? new Dictionary<string, Caller>();

        private static async Task Async(Action action, int milliseconds, string name, bool isInterval = false)
        {
            ThreadInit();
            if (!callerDict.ContainsKey(name)) callerDict[name] = new Caller();
            var caller = callerDict[name];
            caller.CancellationTokenSource?.Cancel();
            caller.CancellationTokenSource = new CancellationTokenSource();
            var ct = caller.CancellationTokenSource.Token;
            if (isInterval) milliseconds -= (int)(DateTime.Now - caller.LastTime).TotalMilliseconds;
            if (milliseconds > 0)
            {
                var task = Task.Delay(milliseconds, ct);
                await task;
                if (ct.IsCancellationRequested) throw new TaskCanceledException(task);//处理取消时已经完成延迟任务
            }
            caller.LastTime = DateTime.Now;
            action?.Invoke();
        }

        /// <summary>
        /// 防抖
        /// </summary>
        public static Task DebounceAsync(Action action = null, int milliseconds = 500, [CallerFilePath]string name = "")
            => Async(action, milliseconds, name);

        /// <summary>
        /// 节流
        /// </summary>
        public static Task ThrottleAsync(Action action = null, int milliseconds = 1000, [CallerFilePath]string name = "")
            => Async(action, milliseconds, name, true);

        /// <summary>
        /// 固定时间间隔
        /// </summary>
        [Obsolete("Use ThrottleAsync")]
        public static Task IntervalAsync(Action action = null, int milliseconds = 1000, [CallerFilePath]string name = "")
            => ThrottleAsync(action, milliseconds, name);

        /// <summary>
        /// 重置延迟时间
        /// </summary>
        [Obsolete("Use DebounceAsync")]
        public static Task ResetAsync(Action action = null, int milliseconds = 500, [CallerFilePath]string name = "")
            => DebounceAsync(action, milliseconds, name);
    }
}
