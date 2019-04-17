using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Vikings.Wpf
{
    static class NativeMethods
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("User32")]
        public static extern int EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("User32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("User32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("User32", BestFitMapping = false, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);
    }

    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public int X => Left;
        public int Y => Top;
        public int Width => Right - Left;
        public int Height => Bottom - Top;
    }
}
