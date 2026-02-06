using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Timers;
using NetworkMonitor.Models;

namespace NetworkMonitor.Services
{
    public class NetworkService : IDisposable
    {
        private Timer? _timer;
        private long _lastBytesReceived;
        private long _lastBytesSent;
        private DateTime _lastTime;
        private bool _isFirstReading = true;

        public event EventHandler<NetworkSpeedEventArgs>? SpeedUpdated;

        public double DownloadSpeed { get; private set; }
        public double UploadSpeed { get; private set; }

        public void Start(int intervalMs = 1000)
        {
            Stop();

            _isFirstReading = true;
            _timer = new Timer(intervalMs);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();

            // Initial reading
            UpdateSpeed();
        }

        public void Stop()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }

        public void SetInterval(int intervalMs)
        {
            if (_timer != null)
            {
                _timer.Interval = intervalMs;
            }
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            UpdateSpeed();
        }

        private void UpdateSpeed()
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                                 ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                 ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel);

                long totalBytesReceived = 0;
                long totalBytesSent = 0;

                foreach (var ni in interfaces)
                {
                    var stats = ni.GetIPv4Statistics();
                    totalBytesReceived += stats.BytesReceived;
                    totalBytesSent += stats.BytesSent;
                }

                var currentTime = DateTime.Now;

                if (_isFirstReading)
                {
                    _isFirstReading = false;
                }
                else
                {
                    var timeDiff = (currentTime - _lastTime).TotalSeconds;
                    if (timeDiff > 0)
                    {
                        // Calculate bytes per second
                        DownloadSpeed = (totalBytesReceived - _lastBytesReceived) / timeDiff;
                        UploadSpeed = (totalBytesSent - _lastBytesSent) / timeDiff;

                        // Ensure non-negative values
                        DownloadSpeed = Math.Max(0, DownloadSpeed);
                        UploadSpeed = Math.Max(0, UploadSpeed);

                        SpeedUpdated?.Invoke(this, new NetworkSpeedEventArgs(DownloadSpeed, UploadSpeed));
                    }
                }

                _lastBytesReceived = totalBytesReceived;
                _lastBytesSent = totalBytesSent;
                _lastTime = currentTime;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading network stats: {ex.Message}");
            }
        }

        public static string FormatSpeed(double bytesPerSecond, SpeedUnit unit)
        {
            double value;
            string unitStr;

            switch (unit)
            {
                case SpeedUnit.Bps:
                    value = bytesPerSecond * 8;
                    unitStr = "bps";
                    break;
                case SpeedUnit.Kbps:
                    value = bytesPerSecond * 8 / 1024;
                    unitStr = "Kbps";
                    break;
                case SpeedUnit.Mbps:
                    value = bytesPerSecond * 8 / 1024 / 1024;
                    unitStr = "Mbps";
                    break;
                case SpeedUnit.Gbps:
                    value = bytesPerSecond * 8 / 1024 / 1024 / 1024;
                    unitStr = "Gbps";
                    break;
                case SpeedUnit.BPS:
                    value = bytesPerSecond;
                    unitStr = "B/s";
                    break;
                case SpeedUnit.KBPS:
                    value = bytesPerSecond / 1024;
                    unitStr = "KB/s";
                    break;
                case SpeedUnit.MBPS:
                    value = bytesPerSecond / 1024 / 1024;
                    unitStr = "MB/s";
                    break;
                case SpeedUnit.GBPS:
                    value = bytesPerSecond / 1024 / 1024 / 1024;
                    unitStr = "GB/s";
                    break;
                default:
                    value = bytesPerSecond * 8 / 1024;
                    unitStr = "Kbps";
                    break;
            }

            // Format based on value size
            if (value >= 100)
                return $"{value:F0} {unitStr}";
            else if (value >= 10)
                return $"{value:F1} {unitStr}";
            else
                return $"{value:F2} {unitStr}";
        }

        public void Dispose()
        {
            Stop();
        }
    }

    public class NetworkSpeedEventArgs : EventArgs
    {
        public double DownloadBytesPerSecond { get; }
        public double UploadBytesPerSecond { get; }

        public NetworkSpeedEventArgs(double download, double upload)
        {
            DownloadBytesPerSecond = download;
            UploadBytesPerSecond = upload;
        }
    }
}
