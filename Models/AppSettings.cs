using System.Windows;

namespace NetworkMonitor.Models
{
    public class AppSettings
    {
        // Position
        public double WindowLeft { get; set; } = 100;
        public double WindowTop { get; set; } = 100;
        public double WindowWidth { get; set; } = 160;
        public double WindowHeight { get; set; } = 100;

        // Display
        public bool AlwaysOnTop { get; set; } = true;
        public bool IsVerticalLayout { get; set; } = true;
        public double Opacity { get; set; } = 0.9;
        public double FontSize { get; set; } = 14;

        // Background
        public BackgroundStyle BackgroundStyle { get; set; } = BackgroundStyle.Blur;
        public string BackgroundColor { get; set; } = "#1E1E1E";

        // Speed Unit
        public SpeedUnit SpeedUnit { get; set; } = SpeedUnit.Kbps;

        // Colors
        public string DownloadColor { get; set; } = "#00D4AA"; // Green-Blue
        public string UploadColor { get; set; } = "#FF6B6B";   // Orange-Red

        // Behavior
        public bool MinimizeToTray { get; set; } = true;
        public bool StartMinimized { get; set; } = false;
        public bool StartWithWindows { get; set; } = false;

        // Refresh Rate (ms)
        public int RefreshRate { get; set; } = 1000;
    }

    public enum SpeedUnit
    {
        Bps,    // Bits per second
        Kbps,   // Kilobits per second
        Mbps,   // Megabits per second
        Gbps,   // Gigabits per second
        BPS,    // Bytes per second
        KBPS,   // Kilobytes per second
        MBPS,   // Megabytes per second
        GBPS    // Gigabytes per second
    }

    public enum BackgroundStyle
    {
        Blur,
        Solid,
        Transparent
    }

    public static class ColorPresets
    {
        public static readonly (string Name, string Download, string Upload)[] Presets = new[]
        {
            ("Ocean", "#00D4AA", "#FF6B6B"),      // Default - Green-Blue / Orange-Red
            ("Neon", "#00FF88", "#FF00FF"),        // Neon Green / Magenta
            ("Sunset", "#FFD700", "#FF4500"),      // Gold / Orange Red
            ("Ice", "#00BFFF", "#FF69B4"),         // Deep Sky Blue / Hot Pink
            ("Forest", "#32CD32", "#FFA500"),      // Lime Green / Orange
            ("Galaxy", "#9370DB", "#FF1493"),      // Medium Purple / Deep Pink
            ("Classic", "#00FF00", "#FF0000"),     // Green / Red
            ("Cyan", "#00FFFF", "#FF8C00"),        // Cyan / Dark Orange
        };
    }
}
