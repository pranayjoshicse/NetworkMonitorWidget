using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using NetworkMonitor.Helpers;
using NetworkMonitor.Models;
using NetworkMonitor.Services;

namespace NetworkMonitor
{
    public partial class MainWindow : Window
    {
        private readonly NetworkService _networkService;
        private readonly SettingsService _settingsService;
        private bool _isExiting = false;

        public MainWindow()
        {
            InitializeComponent();
            
            _networkService = new NetworkService();
            _settingsService = SettingsService.Instance;
            
            _networkService.SpeedUpdated += OnSpeedUpdated;
            _settingsService.SettingsChanged += OnSettingsChanged;
            
            // Set tray icon
            TrayIcon.Icon = IconHelper.CreateIcon();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Apply saved settings
            ApplySettings();
            
            // Set window position
            var settings = _settingsService.Settings;
            Left = settings.WindowLeft;
            Top = settings.WindowTop;
            Width = settings.WindowWidth;
            Height = settings.WindowHeight;

            // Ensure window is on screen
            EnsureWindowOnScreen();

            // Apply Windows 11 rounded corners
            WindowHelper.SetRoundedCorners(this);
            
            // Hide from Alt+Tab
            WindowHelper.HideFromAltTab(this);

            // Apply blur effect
            ApplyBackgroundStyle();

            // Start monitoring
            _networkService.Start(_settingsService.Settings.RefreshRate);

            // Start minimized if configured
            if (settings.StartMinimized)
            {
                Hide();
            }

            // Play fade in animation
            BeginStoryboard((System.Windows.Media.Animation.Storyboard)FindResource("FadeIn"));
        }

        private void EnsureWindowOnScreen()
        {
            var screenWidth = SystemParameters.VirtualScreenWidth;
            var screenHeight = SystemParameters.VirtualScreenHeight;
            var screenLeft = SystemParameters.VirtualScreenLeft;
            var screenTop = SystemParameters.VirtualScreenTop;

            if (Left < screenLeft) Left = screenLeft;
            if (Top < screenTop) Top = screenTop;
            if (Left + Width > screenLeft + screenWidth) Left = screenLeft + screenWidth - Width;
            if (Top + Height > screenTop + screenHeight) Top = screenTop + screenHeight - Height;
        }

        private void ApplySettings()
        {
            var settings = _settingsService.Settings;

            // Topmost
            Topmost = settings.AlwaysOnTop;
            AlwaysOnTopMenuItem.IsChecked = settings.AlwaysOnTop;

            // Layout
            bool isVertical = settings.IsVerticalLayout;
            VerticalPanel.Visibility = isVertical ? Visibility.Visible : Visibility.Collapsed;
            HorizontalPanel.Visibility = isVertical ? Visibility.Collapsed : Visibility.Visible;
            VerticalLayoutMenuItem.IsChecked = isVertical;
            HorizontalLayoutMenuItem.IsChecked = !isVertical;

            // Opacity
            Opacity = settings.Opacity;

            // Font Size
            UpdateFontSize(settings.FontSize);

            // Colors
            ApplyColors();
        }

        private void ApplyColors()
        {
            var settings = _settingsService.Settings;
            var downloadBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.DownloadColor));
            var uploadBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.UploadColor));

            // Vertical layout
            DownloadIcon.Foreground = downloadBrush;
            DownloadSpeedText.Foreground = downloadBrush;
            UploadIcon.Foreground = uploadBrush;
            UploadSpeedText.Foreground = uploadBrush;

            // Horizontal layout
            DownloadIconH.Foreground = downloadBrush;
            DownloadSpeedTextH.Foreground = downloadBrush;
            UploadIconH.Foreground = uploadBrush;
            UploadSpeedTextH.Foreground = uploadBrush;
        }

        private void UpdateFontSize(double fontSize)
        {
            DownloadIcon.FontSize = fontSize;
            DownloadSpeedText.FontSize = fontSize;
            UploadIcon.FontSize = fontSize;
            UploadSpeedText.FontSize = fontSize;

            DownloadIconH.FontSize = fontSize;
            DownloadSpeedTextH.FontSize = fontSize;
            UploadIconH.FontSize = fontSize;
            UploadSpeedTextH.FontSize = fontSize;
        }

        private void ApplyBackgroundStyle()
        {
            var settings = _settingsService.Settings;

            switch (settings.BackgroundStyle)
            {
                case BackgroundStyle.Blur:
                    MainBorder.Background = new SolidColorBrush(Color.FromArgb(0x99, 0x1E, 0x1E, 0x1E));
                    try
                    {
                        BlurHelper.EnableBlur(this, 0x99000000);
                    }
                    catch
                    {
                        // Blur not supported, fall back to solid
                        MainBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.BackgroundColor));
                    }
                    break;

                case BackgroundStyle.Solid:
                    BlurHelper.DisableBlur(this);
                    MainBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.BackgroundColor));
                    break;

                case BackgroundStyle.Transparent:
                    BlurHelper.DisableBlur(this);
                    MainBorder.Background = new SolidColorBrush(Color.FromArgb(0x44, 0x1E, 0x1E, 0x1E));
                    break;
            }
        }

        private void OnSpeedUpdated(object? sender, NetworkSpeedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var settings = _settingsService.Settings;
                var downloadText = NetworkService.FormatSpeed(e.DownloadBytesPerSecond, settings.SpeedUnit);
                var uploadText = NetworkService.FormatSpeed(e.UploadBytesPerSecond, settings.SpeedUnit);

                // Update vertical layout
                DownloadSpeedText.Text = downloadText;
                UploadSpeedText.Text = uploadText;

                // Update horizontal layout
                DownloadSpeedTextH.Text = downloadText;
                UploadSpeedTextH.Text = uploadText;

                // Update tray tooltip
                TrayIcon.ToolTipText = $"↓ {downloadText}\n↑ {uploadText}";
            });
        }

        private void OnSettingsChanged(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ApplySettings();
                ApplyBackgroundStyle();
                _networkService.SetInterval(_settingsService.Settings.RefreshRate);
            });
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Double-click to open settings
                OpenSettings();
            }
            else
            {
                DragMove();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Viewbox handles font scaling automatically
            // Save size
            if (IsLoaded)
            {
                _settingsService.Settings.WindowWidth = ActualWidth;
                _settingsService.Settings.WindowHeight = ActualHeight;
            }
        }

        private void Window_LocationChanged(object? sender, EventArgs e)
        {
            if (IsLoaded)
            {
                _settingsService.Settings.WindowLeft = Left;
                _settingsService.Settings.WindowTop = Top;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExiting && _settingsService.Settings.MinimizeToTray)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                // Save settings on exit
                _settingsService.Save();
                _networkService.Dispose();
                TrayIcon.Dispose();
            }
        }

        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow
            {
                Owner = this
            };
            settingsWindow.ShowDialog();
        }

        #region Context Menu Handlers

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            OpenSettings();
        }

        private void AlwaysOnTop_Changed(object sender, RoutedEventArgs e)
        {
            _settingsService.Settings.AlwaysOnTop = AlwaysOnTopMenuItem.IsChecked;
            Topmost = AlwaysOnTopMenuItem.IsChecked;
            _settingsService.Save();
        }

        private void Layout_Changed(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as System.Windows.Controls.MenuItem;
            bool isVertical = menuItem == VerticalLayoutMenuItem;

            VerticalLayoutMenuItem.IsChecked = isVertical;
            HorizontalLayoutMenuItem.IsChecked = !isVertical;

            VerticalPanel.Visibility = isVertical ? Visibility.Visible : Visibility.Collapsed;
            HorizontalPanel.Visibility = isVertical ? Visibility.Collapsed : Visibility.Visible;

            _settingsService.Settings.IsVerticalLayout = isVertical;
            _settingsService.Save();
        }

        private void MinimizeToTray_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _isExiting = true;
            Application.Current.Shutdown();
        }

        #endregion

        #region Tray Icon Handlers

        private void TrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ShowWindow();
        }

        private void TrayShow_Click(object sender, RoutedEventArgs e)
        {
            ShowWindow();
        }

        private void ShowWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        #endregion
    }
}
