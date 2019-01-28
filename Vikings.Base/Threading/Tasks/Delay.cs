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
        [ThreadStatic] static Dictionary<string, Caller> callerDict;

        static void ThreadInit() => callerDict = callerDict ?? new Dictionary<string, Caller>();

        private static async Task Async(Action action, int milliseconds, string name, bool isInterval = false)
        {
            ThreadInit();
            if (!callerDict.ContainsKey(name)) callerDict[name] = new Caller { name = name };
            var caller = callerDict[name];
            caller.cancellationTokenSource?.Cancel();
            caller.cancellationTokenSource = new CancellationTokenSource();
            var ct = caller.cancellationTokenSource.Token;
            if (isInterval) milliseconds -= (int)(DateTime.Now - caller.lastTime).TotalMilliseconds;
            if (milliseconds > 0)
            {
                var task = Task.Delay(milliseconds, ct);
                await task;
                if (ct.IsCancellationRequested) throw new TaskCanceledException(task);//处理取消时已经完成延迟任务
            }
            caller.lastTime = DateTime.Now;
            action?.Invoke();
        }

        /// <summary>
        /// 固定时间间隔
        /// </summary>
        public static Task IntervalAsync(Action action = null, int milliseconds = 1000, [CallerFilePath]string name = "")
            => Async(action, milliseconds, name, true);

        /// <summary>
        /// 重置延迟时间
        /// </summary>
        public static Task ResetAsync(Action action = null, int milliseconds = 500, [CallerFilePath]string name = "")
            => Async(action, milliseconds, name);

        class Caller
        {
            protected internal string name;
            protected internal CancellationTokenSource cancellationTokenSource;
            protected internal DateTime lastTime;
        }
    }
}
