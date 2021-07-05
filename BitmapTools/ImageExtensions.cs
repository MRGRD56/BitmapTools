#if NET5_0_OR_GREATER || NETCOREAPP3_1 || NETSTANDARD2_1
#define NETSTANDARD2_1_COMPATIBLE //.NET 5.0 or .NET Core 3.1 or .NET Standard 2.1
#endif

using System;
using System.Drawing;
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
    }
}