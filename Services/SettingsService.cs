using System;
using System.IO;
using NetworkMonitor.Models;
using Newtonsoft.Json;

namespace NetworkMonitor.Services
{
    public class SettingsService
    {
        private static readonly Lazy<SettingsService> _instance = new(() => new SettingsService());
        public static SettingsService Instance => _instance.Value;

        private readonly string _settingsPath;
        public AppSettings Settings { get; set; }

        public event EventHandler? SettingsChanged;

        private SettingsService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NetworkMonitor"
            );
            
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            _settingsPath = Path.Combine(appDataPath, "settings.json");
            Settings = new AppSettings();
        }

        public void Load()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    Settings = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch
            {
                Settings = new AppSettings();
            }
        }

        public void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                File.WriteAllText(_settingsPath, json);
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }
    }
}
