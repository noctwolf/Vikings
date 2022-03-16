using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

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
        /// if (!this.LogicalAll().OfType&lt;CheckBox&gt;().Any(f => f.IsChecked == true))
        ///     MessageBox.Show("请至少选择一个");
        /// </code>
        /// </example>
        /// <remarks>后续代码可使用Linq</remarks>
        /// <param name="dependencyObject">要枚举的逻辑树节点</param>
        /// <param name="includeParent">是否包含父</param>
        /// <returns>可枚举集合。先子后父、先近后远的顺序</returns>
        public static IEnumerable<DependencyObject> LogicalAll(this DependencyObject dependencyObject, bool includeParent = false)
        {
            yield return dependencyObject;
            foreach (var result in dependencyObject.LogicalChild()) yield return result;
            if (includeParent)
            {
                while (LogicalTreeHelper.GetParent(dependencyObject) is DependencyObject parent)
                {
                    yield return parent;
                    foreach (var result in parent.LogicalChild(dependencyObject)) yield return result;
                    dependencyObject = parent;
                }
            }
        }

        /// <summary>
        /// 枚举逻辑树上的子和父
        /// </summary>
        [Obsolete("Use LogicalAll")]
        public static IEnumerable<DependencyObject> All(this DependencyObject dependencyObject, bool includeParent = false) =>
            dependencyObject.LogicalAll(includeParent);

        /// <summary>
        /// 枚举逻辑树上的子
        /// </summary>
        /// <remarks>后续代码可使用Linq</remarks>
        /// <param name="dependencyObject">要枚举的逻辑树节点</param>
        /// <param name="exclude">内部使用，用于遍历父时排除自己</param>
        /// <returns>可枚举集合。先近后远的顺序</returns>
        public static IEnumerable<DependencyObject> LogicalChild(this DependencyObject dependencyObject, DependencyObject exclude = null)
        {
            foreach (var item in LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>())
                if (item != exclude) yield return item;
            foreach (var item in LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>())
                if (item != exclude) foreach (var result in item.LogicalChild()) yield return result;
        }

        /// <summary>
        /// 枚举逻辑树上的子
        /// </summary>
        [Obsolete("Use LogicalChild")]
        public static IEnumerable<DependencyObject> Child(this DependencyObject dependencyObject, DependencyObject exclude = null) =>
            dependencyObject.LogicalChild(exclude);

        /// <summary>
        /// 枚举可视化树上的子和父
        /// </summary>
        /// <example>示例：
        /// <code>
        /// if (!this.VisualAll().OfType&lt;CheckBox&gt;().Any(f => f.IsChecked == true))
        ///     MessageBox.Show("请至少选择一个");
        /// </code>
        /// </example>
        /// <remarks>后续代码可使用Linq</remarks>
        /// <param name="dependencyObject">要枚举的可视化树节点</param>
        /// <param name="includeParent">是否包含父</param>
        /// <returns>可枚举集合。先子后父、先近后远的顺序</returns>
        public static IEnumerable<DependencyObject> VisualAll(this DependencyObject dependencyObject, bool includeParent = false)
        {
            yield return dependencyObject;
            foreach (var result in dependencyObject.VisualChild()) yield return result;
            if (includeParent)
            {
                while (VisualTreeHelper.GetParent(dependencyObject) is DependencyObject parent)
                {
                    yield return parent;
                    foreach (var result in parent.VisualChild(dependencyObject)) yield return result;
                    dependencyObject = parent;
                }
            }
        }

        /// <summary>
        /// 枚举可视化树上的子
        /// </summary>
        /// <remarks>后续代码可使用Linq</remarks>
        /// <param name="dependencyObject">要枚举的可视化树节点</param>
        /// <param name="exclude">内部使用，用于遍历父时排除自己</param>
        /// <returns>可枚举集合。先近后远的顺序</returns>
        public static IEnumerable<DependencyObject> VisualChild(this DependencyObject dependencyObject, DependencyObject exclude = null)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var item = VisualTreeHelper.GetChild(dependencyObject, i);
                if (item != exclude) yield return item;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var item = VisualTreeHelper.GetChild(dependencyObject, i);
                if (item != exclude) foreach (var result in item.VisualChild()) yield return result;
            }
        }
    }
}
