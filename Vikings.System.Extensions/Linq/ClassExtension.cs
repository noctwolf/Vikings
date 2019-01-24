using System.Collections.Generic;
using System.Reflection;

namespace System.Linq
{
    public static class ClassExtension
    {
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
