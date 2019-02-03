using Microsoft.VisualStudio.PlatformUI;

namespace Vikings.CodeHelper.View
{
    public class BaseDialogWindow : DialogWindow
    {
        public BaseDialogWindow()
        {
            HasMaximizeButton = true;
            HasMinimizeButton = true;
        }
    }
}
