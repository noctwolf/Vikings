using System.Collections.Generic;

namespace System.Windows.Forms
{
    /// <summary>
    /// Control 扩展
    /// </summary>
    public static class ControlExtension
    {
        /// <summary>
        /// 枚举所有子控件的集合
        /// </summary>
        /// <param name="value">要枚举的控件</param>
        /// <param name="includeHasChildren">是否包含非末级节点</param>
        /// <returns>所有子控件集合</returns>
        public static IEnumerable<Control> All(this Control value, bool includeHasChildren = false)
        {
            foreach (Control control in value.Controls)
            {
                if (control.HasChildren)
                {
                    if (includeHasChildren) yield return control;
                    foreach (Control child in control.All(includeHasChildren)) yield return child;
                }
                else
                    yield return control;
            }
        }
    }
}