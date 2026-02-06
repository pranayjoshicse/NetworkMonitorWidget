using System;
using System.Windows;
using NetworkMonitor.Services;

namespace NetworkMonitor
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Ensure settings are loaded
            SettingsService.Instance.Load();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
