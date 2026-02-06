using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using NetworkMonitor.Models;
using NetworkMonitor.Services;

namespace NetworkMonitor
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsService _settingsService;
        private bool _isLoading = true;

        public SettingsWindow()
        {
            InitializeComponent();
            _settingsService = SettingsService.Instance;
            LoadSettings();
        }

        private void LoadSettings()
        {
            _isLoading = true;
            var settings = _settingsService.Settings;

            // Speed Unit
            SpeedUnitCombo.SelectedIndex = (int)settings.SpeedUnit;

            // Layout
            LayoutCombo.SelectedIndex = settings.IsVerticalLayout ? 0 : 1;

            // Background Style
            BackgroundStyleCombo.SelectedIndex = (int)settings.BackgroundStyle;

            // Opacity
            OpacitySlider.Value = settings.Opacity;
            OpacityValue.Text = $"{(int)(settings.Opacity * 100)}%";

            // Color Presets
            ColorPresetCombo.Items.Clear();
            foreach (var preset in ColorPresets.Presets)
            {
                ColorPresetCombo.Items.Add(preset.Name);
            }
            ColorPresetCombo.SelectedIndex = 0; // Default

            // Current Colors
            UpdateColorPreviews(settings.DownloadColor, settings.UploadColor);

            // Behavior
            AlwaysOnTopCheck.IsChecked = settings.AlwaysOnTop;
            MinimizeToTrayCheck.IsChecked = settings.MinimizeToTray;
            StartMinimizedCheck.IsChecked = settings.StartMinimized;
            StartWithWindowsCheck.IsChecked = settings.StartWithWindows;

            // Refresh Rate
            RefreshRateSlider.Value = settings.RefreshRate;
            RefreshRateValue.Text = $"{settings.RefreshRate}ms";

            _isLoading = false;
        }

        private void UpdateColorPreviews(string downloadColor, string uploadColor)
        {
            try
            {
                DownloadColorPreview.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(downloadColor));
                DownloadColorText.Text = downloadColor;
                
                UploadColorPreview.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(uploadColor));
                UploadColorText.Text = uploadColor;
            }
            catch
            {
                // Invalid color, use defaults
            }
        }

        private void SpeedUnitCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading) return;
            _settingsService.Settings.SpeedUnit = (SpeedUnit)SpeedUnitCombo.SelectedIndex;
            _settingsService.Save();
        }

        private void LayoutCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading) return;
            _settingsService.Settings.IsVerticalLayout = LayoutCombo.SelectedIndex == 0;
            _settingsService.Save();
        }

        private void BackgroundStyleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading) return;
            _settingsService.Settings.BackgroundStyle = (BackgroundStyle)BackgroundStyleCombo.SelectedIndex;
            _settingsService.Save();
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isLoading || OpacityValue == null) return;
            _settingsService.Settings.Opacity = OpacitySlider.Value;
            OpacityValue.Text = $"{(int)(OpacitySlider.Value * 100)}%";
            _settingsService.Save();
        }

        private void ColorPresetCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading || ColorPresetCombo.SelectedIndex < 0) return;
            
            var preset = ColorPresets.Presets[ColorPresetCombo.SelectedIndex];
            _settingsService.Settings.DownloadColor = preset.Download;
            _settingsService.Settings.UploadColor = preset.Upload;
            UpdateColorPreviews(preset.Download, preset.Upload);
            _settingsService.Save();
        }

        private void DownloadColorPreview_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = colorDialog.Color;
                var hexColor = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                _settingsService.Settings.DownloadColor = hexColor;
                UpdateColorPreviews(hexColor, _settingsService.Settings.UploadColor);
                _settingsService.Save();
            }
        }

        private void UploadColorPreview_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = colorDialog.Color;
                var hexColor = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                _settingsService.Settings.UploadColor = hexColor;
                UpdateColorPreviews(_settingsService.Settings.DownloadColor, hexColor);
                _settingsService.Save();
            }
        }

        private void AlwaysOnTopCheck_Changed(object sender, RoutedEventArgs e)
        {
            if (_isLoading) return;
            _settingsService.Settings.AlwaysOnTop = AlwaysOnTopCheck.IsChecked ?? true;
            _settingsService.Save();
        }

        private void MinimizeToTrayCheck_Changed(object sender, RoutedEventArgs e)
        {
            if (_isLoading) return;
            _settingsService.Settings.MinimizeToTray = MinimizeToTrayCheck.IsChecked ?? true;
            _settingsService.Save();
        }

        private void StartMinimizedCheck_Changed(object sender, RoutedEventArgs e)
        {
            if (_isLoading) return;
            _settingsService.Settings.StartMinimized = StartMinimizedCheck.IsChecked ?? false;
            _settingsService.Save();
        }

        private void StartWithWindowsCheck_Changed(object sender, RoutedEventArgs e)
        {
            if (_isLoading) return;
            _settingsService.Settings.StartWithWindows = StartWithWindowsCheck.IsChecked ?? false;
            
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key != null)
                {
                    if (_settingsService.Settings.StartWithWindows)
                    {
                        var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
                        if (!string.IsNullOrEmpty(exePath))
                        {
                            key.SetValue("NetworkMonitor", $"\"{exePath}\"");
                        }
                    }
                    else
                    {
                        key.DeleteValue("NetworkMonitor", false);
                    }
                    key.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to modify startup settings: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            _settingsService.Save();
        }

        private void RefreshRateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isLoading || RefreshRateValue == null) return;
            _settingsService.Settings.RefreshRate = (int)RefreshRateSlider.Value;
            RefreshRateValue.Text = $"{(int)RefreshRateSlider.Value}ms";
            _settingsService.Save();
        }

        private void ResetDefaults_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Reset all settings to default values?", "Reset Settings",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _settingsService.Settings = new AppSettings();
                _settingsService.Save();
                LoadSettings();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
