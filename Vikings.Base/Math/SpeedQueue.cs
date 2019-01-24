using System;
using System.Collections.Generic;
using System.Linq;

namespace Vikings.Math
{
    public class SpeedQueue : Queue<Speed>
    {
        public int BoundedCapacity { get; set; } = 100;

        public void Enqueue(long length) => Enqueue(new Speed { Length = length });

        public new void Enqueue(Speed item)
        {
            if (this.LastOrDefault()?.DateTime > item.DateTime)
                throw new Exception($"{nameof(item.DateTime)}必须大于队列中已有值");
            while (Count >= BoundedCapacity) Dequeue();
            base.Enqueue(item);
        }

        public double LastSpeed(int count = 2)
        {
            if (count < 2) throw new ArgumentException(nameof(count));
            return CalcSpeed(this.Skip(Count - count));
        }

        public double LastTimeSpeed(double seconds = 1) => LastTimeSpeed(TimeSpan.FromSeconds(seconds));

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
