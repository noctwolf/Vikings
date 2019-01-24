using System.Text;

namespace System.Winapi
{
    public static class Shlwapi
    {
        public static string FormatByteSize(long length)
        {
            StringBuilder sb = new StringBuilder(64);
            NativeMethods.StrFormatByteSize(length, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}
