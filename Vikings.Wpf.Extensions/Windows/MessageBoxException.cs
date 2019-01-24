using System.Linq;

namespace System.Windows
{
    public class MessageBoxException
    {
        public MessageBoxException(string messageBoxText, string caption = null, MessageBoxImage icon = MessageBoxImage.None)
        {
            MessageBoxText = messageBoxText;
            Caption = caption ?? Application.Current.MainWindow.Title;
            Icon = icon;
        }

        public string MessageBoxText { get; }

        public string Caption { get; }

        public MessageBoxImage Icon { get; }

        public void Show()
        {
            var owner = Application.Current.Windows.OfType<Window>().Single(f => f.IsActive);
            MessageBox.Show(owner, MessageBoxText, Caption, MessageBoxButton.OK, Icon);
        }
    }
}
