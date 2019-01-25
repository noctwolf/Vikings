using System.Collections.Generic;

namespace System.Windows.Forms
{
    public static class ControlExtension
    {
        public static IEnumerable<Control> All(this Control value, bool includeHasChildren = false)
        {
            foreach (Control control in value.Controls)
            {
                if (control.HasChildren)
                {
                    if (includeHasChildren) yield return control;
                    foreach (Control child in control.All(includeHasChildren)) yield return child;
                }
                else
                    yield return control;
            }
        }
    }
}