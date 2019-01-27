using System.Collections.Generic;
using System.Reflection;

namespace System.Linq
{
    /// <summary>
    /// class 扩展
    /// </summary>
    public static class ClassExtension
    {
        /// <summary>
        /// 对比公共属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">要对比的实例</param>
        /// <param name="to">要对比的另一个实例</param>
        /// <param name="ignore">要忽略的属性</param>
        /// <returns>如果实例相同或实例的公共属性都相同，返回<see langword="true"/>。否则返回<see langword="false"/></returns>
        public static bool PublicInstancePropertiesEqual<T>(this T self, T to, params string[] ignore) where T : class
        {
            if (self != null && to != null)
            {
                var type = typeof(T);
                var ignoreList = new List<string>(ignore ?? new string[] { });
                // Selects the properties which have unequal values into a sequence of those properties.
                var unequalProperties = from pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                        where !ignoreList.Contains(pi.Name) && pi.GetIndexParameters().Length == 0
                                        let selfValue = type.GetProperty(pi.Name).GetValue(self, null)
                                        let toValue = type.GetProperty(pi.Name).GetValue(to, null)
                                        where (selfValue == null || !selfValue.Equals(toValue)) && selfValue != toValue
                                        select new { Prop = pi.Name, selfValue, toValue };
                return !unequalProperties.Any();
            }
            return self == to;
        }
    }
}
