using System.Linq;
using System.Windows.Media;

namespace System.Windows
{
    /// <summary>
    /// Window 扩展
    /// </summary>
    public static class WindowExtension
    {
        /// <summary>
        /// 打开一个窗口，并且仅在新打开的窗口关闭后才返回。
        /// </summary>
        /// <param name="window">要显示的窗口</param>
        /// <param name="owner">窗口的拥有者窗口。如指定窗口的子，会自动找窗口。如指定为空，会使用当前激活窗口</param>
        /// <returns>
        /// <see cref="bool" /> 类型的 <see cref="System.Nullable&lt;T&gt;" /> 值， 该值指定活动被接受 (<see langword="true" />) 还是被取消 (<see langword="false" />)。
        /// 返回值是 <see cref="Window.DialogResult" /> 属性在窗口关闭前具有的值
        /// </returns>
        public static bool? ShowDialog(this Window window, DependencyObject owner)
        {
            if (window.Owner == null)
            {
                while (!(owner == null || owner is Window)) owner = VisualTreeHelper.GetParent(owner);
                window.Owner = owner is Window ownerWindow ? ownerWindow : Application.Current.Windows.OfType<Window>().Single(f => f.IsActive);
            }
            if (window.Owner != null) window.ShowInTaskbar = false;
            return window.ShowDialog();
        }
    }
}
