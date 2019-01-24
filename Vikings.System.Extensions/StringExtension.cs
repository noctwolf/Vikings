using System.Linq;

namespace System
{
    public static class StringExtension
    {
        public static string Substring(this string value, string start, string end)
        {
            int iStart = value.IndexOf(start);
            if (iStart > -1)
            {
                iStart = iStart + start.Length;
                int iEnd = value.IndexOf(end, iStart);
                if (iEnd > -1) return value.Substring(iStart, iEnd - iStart);
            }
            return null;
        }

        public static string ToValidFileName(this string value) => 
            string.Join("", value?.ToCharArray().Where(c => !IO.Path.GetInvalidFileNameChars().Contains(c)));
    }
}