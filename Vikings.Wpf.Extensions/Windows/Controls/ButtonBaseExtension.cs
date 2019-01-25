using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Controls
{
    public static class ButtonBaseExtension
    {
        public static void OnClick(this Primitives.ButtonBase buttonBase)
        {
            buttonBase.GetType().GetMethod(nameof(OnClick), BindingFlags.Instance | BindingFlags.NonPublic).Invoke(buttonBase, null);
        }
    }
}
