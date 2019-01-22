using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<DependencyObject> All(this DependencyObject dependencyObject, bool containsParent = false)
        {
            yield return dependencyObject;
            foreach (var result in dependencyObject.Child()) yield return result;
            if (containsParent)
            {
                while (LogicalTreeHelper.GetParent(dependencyObject) is DependencyObject parent)
                {
                    yield return parent;
                    foreach (var result in parent.Child(dependencyObject)) yield return result;
                    dependencyObject = parent;
                }
            }
        }

        public static IEnumerable<DependencyObject> Child(this DependencyObject dependencyObject, DependencyObject exclude = null)
        {
            foreach (var item in LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>())
                if (item != exclude) yield return item;
            foreach (var item in LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>())
                if (item != exclude) foreach (var result in item.Child()) yield return result;
        }
    }
}
