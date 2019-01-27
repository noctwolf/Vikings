using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    /// <summary>
    /// DependencyObject 扩展
    /// </summary>
    public static class DependencyObjectExtension
    {
        /// <summary>
        /// 枚举逻辑树上的子和父
        /// </summary>
        /// <example>示例：
        /// <code>
        /// if (!this.All().OfType&lt;CheckBox&gt;().Any(f => f.IsChecked == true))
        ///     MessageBox.Show("请至少选择一个");
        /// </code>
        /// </example>
        /// <remarks>后续代码可使用Linq</remarks>
        /// <param name="dependencyObject">要枚举的逻辑树节点</param>
        /// <param name="includeParent">是否包含父</param>
        /// <returns>可枚举集合。先子后父、先近后远的顺序</returns>
        public static IEnumerable<DependencyObject> All(this DependencyObject dependencyObject, bool includeParent = false)
        {
            yield return dependencyObject;
            foreach (var result in dependencyObject.Child()) yield return result;
            if (includeParent)
            {
                while (LogicalTreeHelper.GetParent(dependencyObject) is DependencyObject parent)
                {
                    yield return parent;
                    foreach (var result in parent.Child(dependencyObject)) yield return result;
                    dependencyObject = parent;
                }
            }
        }

        /// <summary>
        /// 枚举逻辑树上的子
        /// </summary>
        /// <remarks>后续代码可使用Linq</remarks>
        /// <param name="dependencyObject">要枚举的逻辑树节点</param>
        /// <param name="exclude">内部使用，用于遍历父时排除自己</param>
        /// <returns>可枚举集合。先近后远的顺序</returns>
        public static IEnumerable<DependencyObject> Child(this DependencyObject dependencyObject, DependencyObject exclude = null)
        {
            foreach (var item in LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>())
                if (item != exclude) yield return item;
            foreach (var item in LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>())
                if (item != exclude) foreach (var result in item.Child()) yield return result;
        }
    }
}
