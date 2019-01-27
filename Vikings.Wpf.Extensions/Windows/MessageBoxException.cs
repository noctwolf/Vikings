using System.Linq;

namespace System.Windows
{
    /// <summary>
    /// 返回并显示消息框，比如用于输入检查
    /// </summary>
    public class MessageBoxException: Exception
    {
        static MessageBoxException()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        static void Current_DispatcherUnhandledException(object sender, Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (!e.Handled && e.Exception is MessageBoxException mbe)
            {
                e.Handled = true;
                mbe.Show();
            }
        }

        /// <summary>
        /// 初始化 <see cref="MessageBoxException" /> 类的新实例
        /// </summary>
        /// <param name="messageBoxText">用于指定要显示的文本</param>
        /// <param name="caption">用于指定要显示的标题栏标题，不指定时取主窗口标题</param>
        /// <param name="icon">用于指定要显示的图标，不指定时不显示图标</param>
        public MessageBoxException(string messageBoxText, string caption = null, MessageBoxImage icon = MessageBoxImage.None)
        {
            MessageBoxText = messageBoxText;
            Caption = caption ?? Application.Current.MainWindow.Title;
            Icon = icon;
        }

        /// <summary>
        /// 要显示的文本
        /// </summary>
        public string MessageBoxText { get; }

        /// <summary>
        /// 要显示的标题栏标题
        /// </summary>
        public string Caption { get; }

        /// <summary>
        /// 要显示的图标
        /// </summary>
        public MessageBoxImage Icon { get; }

        /// <summary>
        /// 显示消息框
        /// </summary>
        public void Show()
        {
            var owner = Application.Current.Windows.OfType<Window>().Single(f => f.IsActive);
            MessageBox.Show(owner, MessageBoxText, Caption, MessageBoxButton.OK, Icon);
        }
    }
}
