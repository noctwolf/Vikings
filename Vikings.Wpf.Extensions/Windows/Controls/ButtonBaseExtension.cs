using System.Reflection;

namespace System.Windows.Controls
{
    /// <summary>
    /// ButtonBase 扩展
    /// </summary>
    public static class ButtonBaseExtension
    {
        /// <summary>
        /// 引发 <see cref="Primitives.ButtonBase.Click" /> 路由事件。
        /// </summary>
        /// <param name="buttonBase">要引发路由事件的按钮</param>
        public static void OnClick(this Primitives.ButtonBase buttonBase)
        {
            buttonBase.GetType().GetMethod(nameof(OnClick), BindingFlags.Instance | BindingFlags.NonPublic).Invoke(buttonBase, null);
        }
    }
}
