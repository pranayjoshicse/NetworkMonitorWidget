using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkMonitor.Helpers
{
    public static class IconHelper
    {
        public static ImageSource CreateTrayIcon()
        {
            using var bitmap = new Bitmap(32, 32);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(System.Drawing.Color.Transparent);

            // Draw download arrow (green)
            using var downloadBrush = new SolidBrush(System.Drawing.Color.FromArgb(0, 212, 170));
            var downloadPoints = new System.Drawing.Point[]
            {
                new(8, 4),
                new(14, 4),
                new(14, 12),
                new(18, 12),
                new(11, 20),
                new(4, 12),
                new(8, 12)
            };
            graphics.FillPolygon(downloadBrush, downloadPoints);

            // Draw upload arrow (red)
            using var uploadBrush = new SolidBrush(System.Drawing.Color.FromArgb(255, 107, 107));
            var uploadPoints = new System.Drawing.Point[]
            {
                new(21, 28),
                new(28, 20),
                new(24, 20),
                new(24, 12),
                new(18, 12),
                new(18, 20),
                new(14, 20)
            };
            graphics.FillPolygon(uploadBrush, uploadPoints);

            var hBitmap = bitmap.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        public static Icon CreateIcon()
        {
            using var bitmap = new Bitmap(32, 32);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(System.Drawing.Color.Transparent);

            // Draw download arrow (green)
            using var downloadBrush = new SolidBrush(System.Drawing.Color.FromArgb(0, 212, 170));
            var downloadPoints = new System.Drawing.Point[]
            {
                new(8, 2),
                new(12, 2),
                new(12, 10),
                new(16, 10),
                new(10, 18),
                new(4, 10),
                new(8, 10)
            };
            graphics.FillPolygon(downloadBrush, downloadPoints);

            // Draw upload arrow (red-orange)
            using var uploadBrush = new SolidBrush(System.Drawing.Color.FromArgb(255, 107, 107));
            var uploadPoints = new System.Drawing.Point[]
            {
                new(22, 30),
                new(28, 22),
                new(24, 22),
                new(24, 14),
                new(20, 14),
                new(20, 22),
                new(16, 22)
            };
            graphics.FillPolygon(uploadBrush, uploadPoints);

            IntPtr hIcon = bitmap.GetHicon();
            return Icon.FromHandle(hIcon);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
    }
}
