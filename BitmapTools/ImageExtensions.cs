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
    public static class ImageExtensions
    {
        private static readonly ImageFormat DefaultImageFormat = ImageFormat.Png;
        
        public static MemoryStream GetStream(this Image image) =>
            image.GetStream(DefaultImageFormat);
        public static MemoryStream GetStream(this Image image, ImageFormat imageFormat)
        {
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, imageFormat);
            return memoryStream;
        }
        
        public static
            #if NETSTANDARD2_1_COMPATIBLE
            Memory<byte>
            #else
            byte[]
            #endif
            GetBytes(this Image image) =>
            image.GetBytes(DefaultImageFormat);
        
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

        public static async Task<Memory<byte>> GetBytesAsync(this Image image) =>
            await image.GetBytesAsync(DefaultImageFormat);
        
        public static async Task<Memory<byte>> GetBytesAsync(this Image image, ImageFormat imageFormat)
        {
            await using var memoryStream = image.GetStream();
            return memoryStream.ToArray();
        }
        
        #endif
    }
}