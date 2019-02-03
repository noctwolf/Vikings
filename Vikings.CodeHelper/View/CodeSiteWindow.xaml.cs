using System;
using System.Linq;
using System.Windows;

namespace Vikings.CodeHelper.View
{
    /// <summary>
    /// CodeSiteWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CodeSiteWindow : BaseDialogWindow
    {
        public CodeSiteWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
