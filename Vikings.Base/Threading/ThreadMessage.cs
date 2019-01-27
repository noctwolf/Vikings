using System.Windows.Interop;
using static Vanara.PInvoke.User32_Gdi;

namespace System.Threading
{
    public static class ThreadMessage
    {
        public static void DoEvents()
        {
            //MSG mSG = default(MSG);
            //while (PeekMessage(ref mSG, NullHandleRef, 0, 0, 1)) DispatchMessage(ref mSG);
        }
    }
}
