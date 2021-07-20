using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using BitmapTools.Wpf;
using Size = System.Drawing.Size;

namespace BitmapTools.WpfTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly FontFamily DefaultFontFamily = System.Drawing.FontFamily.GenericSansSerif;
        
        public MainWindow()
        {
            InitializeComponent();

            var bitmap = new Bitmap(150, 100);
            using (var bitmapGraphics = Graphics.FromImage(bitmap))
            {
                bitmapGraphics.Clear(ColorFactory.CreateColorFromHex("0x1e88e5"));
                var textFont = new Font(DefaultFontFamily, 16, System.Drawing.FontStyle.Bold);
                var fgBrush = new SolidBrush(ColorFactory.CreateColorFromArgb(0xffffeb3b));
                bitmapGraphics.DrawString(DateTime.Now.ToShortDateString(), textFont, fgBrush, new PointF(0, 0));
            }

            var bitmapSource = bitmap.ToBitmapSource();
            MainImage.Source = bitmapSource;
            var convertedBitmap = bitmapSource.ToBitmap();
            bitmap.Save("original.png", ImageFormat.Png);
            convertedBitmap.Save("converted.png", ImageFormat.Png);
            bitmap.Resize(75, 50).Save("resized.png", ImageFormat.Png);
            bitmap.SaveCompressed("compressed1.jpg", 10);
            bitmap.Compress(10).Save("compressed2.jpg", ImageFormat.Jpeg);

            var screenSize = new Size((int) SystemParameters.VirtualScreenWidth, (int) SystemParameters.VirtualScreenHeight);
            var screenshot = new Bitmap(screenSize.Width, screenSize.Height);
            using (var screenshotGraphics = Graphics.FromImage(screenshot))
            {
                screenshotGraphics.CopyFromScreen(0, 0, 0, 0, screenSize);
            }
            
            screenshot.Save("screen.jpg", ImageFormat.Jpeg);
            screenshot.Resize(2).Save("screen_x2.jpg", ImageFormat.Jpeg);
            screenshot.Resize(0.5).Save("screen_x05.jpg", ImageFormat.Jpeg);

            var lightWeightScreenshot1 = screenshot
                .Resize(1280, 720);
            
            lightWeightScreenshot1.Save("screenshot_lw_1.jpg", ImageFormat.Jpeg);
            
            var lightWeightScreenshot2 = screenshot
                .Resize(1280, 720)
                .Compress(50);
            
            lightWeightScreenshot2.Save("screenshot_lw_2.jpg", ImageFormat.Jpeg);
            
            Close();
        }
    }
}
