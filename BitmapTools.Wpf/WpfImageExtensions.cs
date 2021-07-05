#if NET5_0_OR_GREATER || NETCOREAPP3_1 || NETSTANDARD2_1
#define NETSTANDARD2_1_COMPATIBLE //.NET 5.0 or .NET Core 3.1 or .NET Standard 2.1
#endif

using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BitmapTools.Wpf
{
    /// <summary>
    /// Provides extensions for WPF images.
    /// </summary>
    public static class WpfImageExtensions
    {
        /// <summary>
        /// Converts the <paramref name="image"/> to a <see cref="BitmapSource"/>.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static BitmapSource ToBitmapSource(this Image image)
        {
            using var imageStream = image.GetStream();
            var imageSource = ConvertToImageSource(imageStream);
            return imageSource;
        }
        
        #if NETSTANDARD2_1_COMPATIBLE

        /// <summary>
        /// Converts the <paramref name="image"/> to a <see cref="BitmapSource"/> in asynchronous mode.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static async Task<BitmapSource> ToBitmapSourceAsync(this Image image)
        {
            await using var imageStream = image.GetStream();
            var imageSource = ConvertToImageSource(imageStream);
            return imageSource;
        }
        
        #endif

        /// <summary>
        /// Converts the <paramref name="bitmapSource"/> to a <see cref="Bitmap"/> in a format defined by <typeparamref name="TEncoder"/>.
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <typeparam name="TEncoder"></typeparam>
        /// <returns></returns>
        public static Bitmap ToBitmap<TEncoder>(this BitmapSource bitmapSource)
            where TEncoder : BitmapEncoder, new()
        {
            using var memoryStream = new MemoryStream();
            return ConvertToBitmap<TEncoder>(bitmapSource, memoryStream);
        }

        /// <summary>
        /// Converts the <paramref name="bitmapSource"/> to a <see cref="Bitmap"/> in PNG format.
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this BitmapSource bitmapSource) =>
            bitmapSource.ToBitmap<PngBitmapEncoder>();
        
        #if NETSTANDARD2_1_COMPATIBLE

        /// <summary>
        /// Converts the <paramref name="bitmapSource"/> to a <see cref="Bitmap"/> in a format defined by <typeparamref name="TEncoder"/> in asynchronous mode.
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <typeparam name="TEncoder"></typeparam>
        /// <returns></returns>
        public static async Task<Bitmap> ToBitmapAsync<TEncoder>(this BitmapSource bitmapSource)
            where TEncoder : BitmapEncoder, new()
        {
            await using var memoryStream = new MemoryStream();
            return ConvertToBitmap<TEncoder>(bitmapSource, memoryStream);
        }

        /// <summary>
        /// Converts the <paramref name="bitmapSource"/> to a <see cref="Bitmap"/> in PNG format in asynchronous mode.
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        public static async Task<Bitmap> ToBitmapAsync(this BitmapSource bitmapSource) => 
            await bitmapSource.ToBitmapAsync<PngBitmapEncoder>();
        
        #endif
        
        private static BitmapSource ConvertToImageSource(Stream imageStream)
        {
            imageStream.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = imageStream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            
            return bitmapImage;
        }
        
        private static Bitmap ConvertToBitmap<TEncoder>(BitmapSource bitmapSource, Stream outStream) 
            where TEncoder : BitmapEncoder, new()
        {
            var encoder = new TEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(outStream);
            return new Bitmap(outStream);
        }
    }
}