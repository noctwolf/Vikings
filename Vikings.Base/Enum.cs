using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Vikings
{
    /// <summary>
    /// enum 扩展
    /// </summary>
    public static class Enum
    {
        /// <summary>
        /// 获取<see langword="enum"/>包含描述的Dictionary
        /// </summary>
        /// <typeparam name="TKey"><see langword="enum"/>类型</typeparam>
        /// <returns>包含描述的Dictionary</returns>
        public static Dictionary<TKey, string> ToDictionary<TKey>() where TKey : System.Enum =>
            typeof(TKey).GetEnumValues().OfType<TKey>().ToDictionary(f => f, f =>
                typeof(TKey).GetMember(f.ToString())[0].GetCustomAttribute<DescriptionAttribute>()?.Description ?? f.ToString());
    }
}
