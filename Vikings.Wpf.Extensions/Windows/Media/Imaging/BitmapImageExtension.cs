using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace System.Windows.Media.Imaging
{
    /// <summary>
    /// BitmapImage 扩展
    /// </summary>
    public static class BitmapImageExtension
    {
        /// <summary>
        /// <see cref="Image"/>转换为<see cref="BitmapImage"/>
        /// </summary>
        /// <param name="value">要转换的图片</param>
        /// <param name="imageFormat">为空时，使用<see cref="ImageFormat.Png"/></param>
        /// <returns></returns>
        public static BitmapImage ToBitmapImage(this Image value, ImageFormat imageFormat = null)
        {
            using (var memoryStream = new MemoryStream())
            {
                value.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }
    }
}
