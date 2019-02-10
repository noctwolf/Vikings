namespace System.Windows
{
    /// <summary>
    /// 用户取消，比如用于在<see cref="Controls.Primitives.ButtonBase.OnClick"/>中取消<see cref="Controls.Primitives.ButtonBase.Command"/>的执行
    /// </summary>
    public class UserCanceledException: Exception
    {
        static UserCanceledException()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        static void Current_DispatcherUnhandledException(object sender, Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is UserCanceledException) e.Handled = true;
        }
    }
}
