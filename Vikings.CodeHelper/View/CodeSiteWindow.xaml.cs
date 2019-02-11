using EnvDTE;
using System.Windows;
using System.Windows.Controls;
using Vikings.CodeHelper.ViewModel;

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

        private void TreeViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem && treeViewItem.DataContext is CodeElementViewModel codeElementViewModel &&
                codeElementViewModel.IsCodeFunction)
            {
                CodeFunction codeFunction = codeElementViewModel.CodeFunction;
                TextPoint textPoint;
                try { textPoint = codeFunction.GetStartPoint(vsCMPart.vsCMPartBody); }
                catch { textPoint = codeFunction.GetStartPoint(); }
                (codeFunction.DTE.ActiveDocument.Selection as TextSelection).MoveToPoint(textPoint);
            }
        }

        private void TreeView_Selected(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TreeViewItem treeViewItem)
            {
                treeViewItem.BringIntoView();
                e.Handled = true;
            }
        }
    }
}
