using System.Linq;

namespace System.Windows
{
    public static class WindowExtension
    {
        public static bool? ShowDialog(this Window window, FrameworkElement owner)
        {
            while (!(owner != null && owner is Window)) owner = owner.Parent as FrameworkElement;
            owner = owner ?? Application.Current.Windows.OfType<Window>().Single(f => f.IsActive);
            window.Owner = owner as Window;
            return window.ShowDialog();
        }
    }
}
