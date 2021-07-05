using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using BitmapTools.Wpf;

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
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(ColorFactory.CreateColorFromHex("0x1e88e5"));
            var textFont = new Font(DefaultFontFamily, 16, System.Drawing.FontStyle.Bold);
            var fgBrush = new SolidBrush(ColorFactory.CreateColorFromArgb(0xffffeb3b));
            graphics.DrawString(DateTime.Now.ToShortDateString(), textFont, fgBrush, new PointF(0, 0));

            var bitmapSource = bitmap.ToBitmapSource();
            MainImage.Source = bitmapSource;
            var convertedBitmap = bitmapSource.ToBitmap();
            bitmap.Save("original.png", ImageFormat.Png);
            convertedBitmap.Save("converted.png", ImageFormat.Png);
        }
    }
}
