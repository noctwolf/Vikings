using System.Collections.Generic;

namespace System
{
    public static class Int32Extension
    {
        public static IEnumerable<int> Split(this int total, int count)
        {
            if (total < 1) throw new ArgumentException("参数值必须大于零", nameof(total));
            if (count < 1) throw new ArgumentException("参数值必须大于零", nameof(count));
            double split = total * 1d / count;
            for (int i = 0; i < count; i++)
                yield return Convert.ToInt32(split * (i + 1)) - Convert.ToInt32(split * i);
        }

        public static string ToByteSize(this int value)
        {
            return Winapi.Shlwapi.FormatByteSize(value);
        }
    }
}
