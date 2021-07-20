#if NET5_0 || NET5_0_OR_GREATER || NETCOREAPP3_1 || NETSTANDARD2_1
#define NETSTANDARD2_1_COMPATIBLE //.NET 5.0 or .NET Core 3.1 or .NET Standard 2.1
#endif

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
            // return resizeMode switch
            // {
            //     ImageResizeMode.HighQuality => ResizeHQ(image, width, height),
            //     ImageResizeMode.StandardQuality => ResizeLQ(image, width, height),
            //     _ => throw new ArgumentOutOfRangeException(nameof(resizeMode), resizeMode, null)
            // };

            return ResizeHQ(image, width, height);
        }

        /// <summary>
        /// Resizes the <paramref name="image"/> to the specified <paramref name="size"/>.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="size">The size to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap Resize(this Image image, Size size) =>
            image.Resize(size.Width, size.Height);

        /// <summary>
        /// Multiplies the size of the <paramref name="image"/> by a <paramref name="sizeMultiplier"/>.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="sizeMultiplier">The value by which the size of the <paramref name="image"/> will be multiplied.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap Resize(this Image image, double sizeMultiplier)
        {
            if (sizeMultiplier < 0)
            {
                throw new ArgumentException("Size multiplier cannot be less than zero", nameof(sizeMultiplier));
            }

            var originalImageSize = image.Size;
            var resizedImageSize = new Size(
                (int) Math.Round(originalImageSize.Width * sizeMultiplier),
                (int) Math.Round(originalImageSize.Height * sizeMultiplier));

            return image.Resize(resizedImageSize);
        }

        /// <summary>
        /// Compresses the <paramref name="image"/> to the specified <paramref name="quality"/> level using the JPEG format.
        /// </summary>
        /// <param name="image">The image to compress.</param>
        /// <param name="quality">The compressed image quality. From 0 to 100.</param>
        /// <returns>The compressed image.</returns>
        public static Bitmap Compress(this Image image, long quality)
        {
            var compressedImageStream = new MemoryStream();
            image.SaveCompressed(compressedImageStream, quality);

            return new Bitmap(compressedImageStream);
        }

        /// <summary>
        /// Compresses the <paramref name="image"/> to the specified <paramref name="quality"/> and saves it in JPEGs format.
        /// </summary>
        /// <param name="image">The image to compress and save.</param>
        /// <param name="stream">The stream to which the image will be saved.</param>
        /// <param name="quality">The compressed image quality. From 0 to 100.</param>
        public static void SaveCompressed(this Image image, Stream stream, long quality)
        {
            image.Save(stream, GetEncoder(ImageFormat.Jpeg), GetEncoderParameters(quality));
        }

        /// <summary>
        /// Compresses the <paramref name="image"/> to the specified <paramref name="quality"/> and saves it in JPEGs format.
        /// </summary>
        /// <param name="image">The image to compress and save.</param>
        /// <param name="fileName">The name of the file where the image will be saved.</param>
        /// <param name="quality">The compressed image quality. From 0 to 100.</param>
        public static void SaveCompressed(this Image image, string fileName, long quality)
        {
            image.Save(fileName, GetEncoder(ImageFormat.Jpeg), GetEncoderParameters(quality));
        }

        /// <summary>
        /// Compresses the <paramref name="image"/> to the specified <paramref name="quality"/> and saves it in the specified format.
        /// </summary>
        /// <param name="image">The image to compress and save.</param>
        /// <param name="stream">The stream to which the image will be saved.</param>
        /// <param name="quality">The compressed image quality. From 0 to 100.</param>
        /// <param name="imageFormat">The image format.</param>
        public static void SaveCompressed(this Image image, Stream stream, long quality, ImageFormat imageFormat)
        {
            image.Save(stream, GetEncoder(imageFormat), GetEncoderParameters(quality));
        }

        /// <summary>
        /// Compresses the <paramref name="image"/> to the specified <paramref name="quality"/> and saves it in the specified format.
        /// </summary>
        /// <param name="image">The image to compress and save.</param>
        /// <param name="fileName">The name of the file where the image will be saved.</param>
        /// <param name="quality">The compressed image quality. From 0 to 100.</param>
        /// <param name="imageFormat">The image format.</param>
        public static void SaveCompressed(this Image image, string fileName, long quality, ImageFormat imageFormat)
        {
            image.Save(fileName, GetEncoder(imageFormat), GetEncoderParameters(quality));
        }

        /// <summary>
        /// Changes the <paramref name="pixelFormat"/> for the <paramref name="image"/>.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="pixelFormat"></param>
        /// <returns></returns>
        public static Bitmap ChangePixelFormat(this Image image, PixelFormat pixelFormat)
        {
            var bmp = new Bitmap(image.Width, image.Height, pixelFormat);
            using var graphics = Graphics.FromImage(bmp);
            graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
            return bmp;
        }
        
        private static Bitmap ResizeHQ(Image image, int width, int height)
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
            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

            return destImage;
        }

        private static Bitmap ResizeLQ(Image image, int width, int height)
        {
            return new(image, new Size(width, height));
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        private static EncoderParameters GetEncoderParameters(long quality)
        {
            return new()
            {
                Param = new[]
                {
                    new EncoderParameter(Encoder.Quality, quality)
                }
            };
        }
    }
}