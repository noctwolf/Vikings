namespace System.Windows
{
    public class UserCanceledException: Exception
    {
        static UserCanceledException()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private static void Current_DispatcherUnhandledException(object sender, Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is UserCanceledException) e.Handled = true;
        }
    }
}
