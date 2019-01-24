using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;

namespace System.Winapi
{
    public class NativeMethods
    {
        public static HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage([In] [Out] ref MSG msg, HandleRef hwnd, int msgMin, int msgMax, int remove);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr DispatchMessage([In] ref MSG msg);

        [DllImport("shlwapi", CharSet = CharSet.Unicode, EntryPoint = "StrFormatByteSizeW")]
        public static extern IntPtr StrFormatByteSize(long qdw, StringBuilder pszBuf, int cchBuf);

        [DllImport("user32")]
        public static extern IntPtr GetActiveWindow();
    }
}
