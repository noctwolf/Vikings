using System;
using System.Collections.Generic;
using System.Linq;

namespace Vikings.Math
{
    /// <summary>
    /// 速度记录队列
    /// </summary>
    public class SpeedQueue : Queue<Speed>
    {
        /// <summary>
        /// 队列容量
        /// </summary>
        public int BoundedCapacity { get; set; } = 100;

        /// <summary>
        /// 添加长度
        /// </summary>
        /// <param name="length"></param>
        public void Enqueue(long length) => Enqueue(new Speed { Length = length });

        /// <summary>
        /// 添加速度
        /// </summary>
        /// <param name="item"></param>
        public new void Enqueue(Speed item)
        {
            if (this.LastOrDefault()?.DateTime > item.DateTime)
                throw new Exception($"{nameof(item.DateTime)}必须大于队列中已有值");
            while (Count >= BoundedCapacity) Dequeue();
            base.Enqueue(item);
        }

        /// <summary>
        /// 获取最后速度
        /// </summary>
        /// <param name="count">采样数量</param>
        /// <returns>最后速度</returns>
        public double LastSpeed(int count = 2)
        {
            if (count < 2) throw new ArgumentException(nameof(count));
            return CalcSpeed(this.Skip(Count - count));
        }

        /// <summary>
        /// 获取最后速度
        /// </summary>
        /// <param name="seconds">采样时间，单位（秒）</param>
        /// <returns>最后速度</returns>
        public double LastTimeSpeed(double seconds = 1) => LastTimeSpeed(TimeSpan.FromSeconds(seconds));

        /// <summary>
        /// 获取最后速度
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <returns>最后速度</returns>
        public double LastTimeSpeed(TimeSpan timeSpan)
        {
            var lastTime = this.SkipWhile(f => f.DateTime < DateTime.Now - timeSpan);
            if (lastTime.Count() == 1 && Count > 1) lastTime = this.Skip(Count - 2);
            return CalcSpeed(lastTime);
        }

        double CalcSpeed(IEnumerable<Speed> source)
        {
            if (!source.Skip(2).Any()) return 0;
            return (source.Last().Length - source.First().Length) /
                (source.Last().DateTime - source.First().DateTime).TotalSeconds;
        }
    }
}
