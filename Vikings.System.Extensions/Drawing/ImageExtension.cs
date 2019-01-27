using System.Drawing.Imaging;
using System.IO;

namespace System.Drawing
{
    /// <summary>
    /// Image 扩展
    /// </summary>
    public static class ImageExtension
    {
        /// <summary>
        /// 图片转换为字节数组
        /// </summary>
        /// <param name="value">要转换的图片</param>
        /// <param name="imageFormat">为空时，使用<see cref="ImageFormat.Png"/></param>
        /// <returns>转换后的字节数组</returns>
        public static byte[] ToArray(this Image value, ImageFormat imageFormat = null)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                value.Save(ms, imageFormat ?? ImageFormat.Png);
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 字节数组转为图片
        /// </summary>
        /// <param name="value">要转换的字节数组</param>
        /// <returns><see cref="Image"/>的新实例</returns>
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

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="value">要生成缩略图的实例</param>
        /// <param name="newSize">缩略图大小</param>
        /// <returns>生成的缩略图</returns>
        public static byte[] ToThumbnail(this byte[] value, Size newSize)
        {
            Image image = value.ToImage();
            if (image == null || image.Size.Width <= newSize.Width && image.Size.Height <= newSize.Height) return null;
            Bitmap bmp = new Bitmap(newSize.Width, newSize.Height);
            Size size = image.Size;
            Rectangle rect = Rectangle.Empty;
            float num = Math.Min((float)bmp.Width / size.Width, (float)bmp.Height / size.Height);
            rect.Width = (int)(size.Width * num);
            rect.Height = (int)(size.Height * num);
            rect.X = (bmp.Width - rect.Width) / 2;
            rect.Y = (bmp.Height - rect.Height) / 2;
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            g.DrawImage(image, rect);
            return bmp.ToArray();
        }
    }
}
