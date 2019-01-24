using System.Drawing.Imaging;
using System.IO;

namespace System.Drawing
{
    public static class ImageExtension
    {
        public static byte[] ToArray(this Image value)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                value.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static Image ToImage(this byte[] value)
        {
            try
            {
                return Image.FromStream(new MemoryStream(value));
            }
            catch
            {
                return null;
            }
        }

        public static bool SequenceEqual(this byte[] value, byte[] other)
        {
            bool imageChanged = false;
            if (value != other && !(imageChanged = value == null || other == null || value.Length != other.Length))
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] != other[i])
                    {
                        imageChanged = true;
                        break;
                    }
                }
            }
            return !imageChanged;
        }

        public static byte[] ToThumbnail(this byte[] value, Size newSize)
        {
            Image image = value.ToImage();
            if (image == null || image.Size.Width <= newSize.Width && image.Size.Height <= newSize.Height) return null;
            Bitmap bmp = new Bitmap(newSize.Width, newSize.Height);
            Size size = image.Size;
            Rectangle rect = Rectangle.Empty;
            float num = Math.Min((float)bmp.Width / (float)size.Width, (float)bmp.Height / (float)size.Height);
            rect.Width = (int)((float)size.Width * num);
            rect.Height = (int)((float)size.Height * num);
            rect.X = (bmp.Width - rect.Width) / 2;
            rect.Y = (bmp.Height - rect.Height) / 2;
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            g.DrawImage(image, rect);
            return bmp.ToArray();
        }
    }
}
