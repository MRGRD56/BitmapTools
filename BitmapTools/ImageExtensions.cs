#if NET5_0_OR_GREATER || NETCOREAPP3_1 || NETSTANDARD2_1
#define NETSTANDARD2_1_COMPATIBLE //.NET 5.0 or .NET Core 3.1 or .NET Standard 2.1
#endif

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace BitmapTools
{
    /// <summary>
    /// Provides extensions for <see cref="System.Drawing.Image"/>.
    /// </summary>
    public static class ImageExtensions
    {
        private static readonly ImageFormat DefaultImageFormat = ImageFormat.Png;
        
        /// <summary>
        /// Creates a stream from the <paramref name="image"/>. The format is PNG.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static MemoryStream GetStream(this Image image) =>
            image.GetStream(DefaultImageFormat);

        /// <summary>
        /// Creates a stream from the <paramref name="image"/>.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageFormat"></param>
        /// <returns></returns>
        public static MemoryStream GetStream(this Image image, ImageFormat imageFormat)
        {
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, imageFormat);
            return memoryStream;
        }
        
        /// <summary>
        /// Returns an array of bytes from the <paramref name="image"/>. The format is PNG.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static
            #if NETSTANDARD2_1_COMPATIBLE
            Memory<byte>
            #else
            byte[]
            #endif
            GetBytes(this Image image) =>
            image.GetBytes(DefaultImageFormat);
        
        /// <summary>
        /// Returns an array of bytes from the <paramref name="image"/>.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageFormat"></param>
        /// <returns></returns>
        public static 
            #if NETSTANDARD2_1_COMPATIBLE
            Memory<byte>
            #else
            byte[]
            #endif
            GetBytes(this Image image, ImageFormat imageFormat)
        {
            using var memoryStream = image.GetStream(imageFormat);
            return memoryStream.ToArray();
        }
        
        #if NETSTANDARD2_1_COMPATIBLE

        /// <summary>
        /// Returns an array of bytes from the <paramref name="image"/> in asynchronous mode. The format is PNG.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static async Task<Memory<byte>> GetBytesAsync(this Image image) =>
            await image.GetBytesAsync(DefaultImageFormat);
        
        /// <summary>
        /// Returns an array of bytes from the <paramref name="image"/> in asynchronous mode.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageFormat"></param>
        /// <returns></returns>
        public static async Task<Memory<byte>> GetBytesAsync(this Image image, ImageFormat imageFormat)
        {
            await using var memoryStream = image.GetStream();
            return memoryStream.ToArray();
        }
        
        #endif
        
        /// <summary>
        /// Resizes the <paramref name="image"/> to the specified <paramref name="width"/> and <paramref name="height"/>.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap Resize(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using var graphics = Graphics.FromImage(destImage);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var wrapMode = new ImageAttributes();
            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);

            return destImage;
        }

        /// <summary>
        /// Resizes the <paramref name="image"/> to the specified <paramref name="size"/>.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="size">The size to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap Resize(this Image image, Size size) => image.Resize(size.Width, size.Height);
    }
}