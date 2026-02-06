# Network Monitor Widget

A lightweight, always-on-top network speed monitor widget for Windows.

## Features

- ✅ **Real-time network monitoring** - Download and upload speeds refreshing every second
- ✅ **Always on top** - Stays visible (can be disabled in settings)
- ✅ **Borderless & draggable** - Clean widget-style design, drag from anywhere
- ✅ **Rounded corners** - Modern Windows 11 style
- ✅ **Blur effect** - Acrylic background (with solid/transparent options)
- ✅ **Multiple speed units** - Kbps, Mbps, KB/s, MB/s, and more
- ✅ **Vibrant colors** - 8 preset color schemes + custom colors
- ✅ **Resizable** - Font scales automatically with window size
- ✅ **System tray** - Minimize to tray with live tooltip
- ✅ **Remembers position** - Window position saved between sessions
- ✅ **Start with Windows** - Optional auto-start

## Requirements

- Windows 10/11
- .NET 8.0 Runtime (or self-contained build)

## Quick Start

```powershell
cd C:\Projects\NetworkMonitor
dotnet run
```

## Installation

### Option 1: Build from Source

1. Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Clone or download this repository
3. Open terminal in the project folder
4. Run:
   ```powershell
   dotnet build -c Release
   ```
5. Find the executable in `bin/Release/net8.0-windows/`

### Option 2: Publish as Single File

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

This creates a single `.exe` file that includes the .NET runtime.

## Usage

### Basic Controls

| Action | Result |
|--------|--------|
| **Left-click + drag** | Move the widget |
| **Double-click** | Open settings |
| **Right-click** | Context menu |
| **Resize grip** | Resize the widget (font scales automatically) |
| **Close button** | Minimize to tray (if enabled) |

### Context Menu Options

- **Settings** - Open settings window
- **Always on Top** - Toggle topmost state
- **Layout** - Switch between vertical/horizontal
- **Minimize to Tray** - Hide to system tray
- **Exit** - Close the application

### System Tray

- The tray icon shows live speeds in the tooltip
- **Double-click** tray icon to show the widget
- **Right-click** tray icon for quick menu

## Settings

### Display
- **Speed Unit**: bps, Kbps, Mbps, Gbps, B/s, KB/s, MB/s, GB/s
- **Layout**: Vertical or Horizontal
- **Background**: Blur (Acrylic), Solid, or Transparent
- **Opacity**: 30% to 100%

### Colors
**Preset Color Schemes:**
| Preset | Download | Upload |
|--------|----------|--------|
| Ocean (default) | Cyan-Green | Coral-Red |
| Neon | Neon Green | Magenta |
| Sunset | Gold | Orange Red |
| Ice | Sky Blue | Hot Pink |
| Forest | Lime Green | Orange |
| Galaxy | Purple | Deep Pink |
| Classic | Green | Red |
| Cyan | Cyan | Dark Orange |

Custom colors can be selected by clicking the color preview boxes.

### Behavior
- **Always on top** - Widget stays above other windows
- **Minimize to tray** - Close button minimizes instead of exiting
- **Start minimized** - Launch hidden in system tray
- **Start with Windows** - Auto-start on login
- **Refresh rate** - 500ms to 5000ms

## Technical Details

- **Framework**: .NET 8.0 WPF
- **Memory usage**: ~15-25 MB
- **CPU usage**: < 1%
- **Dependencies**: 
  - Hardcodet.NotifyIcon.Wpf (system tray)
  - Newtonsoft.Json (settings persistence)

## File Structure

```
NetworkMonitor/
├── NetworkMonitor.csproj
├── App.xaml / App.xaml.cs
├── MainWindow.xaml / MainWindow.xaml.cs
├── SettingsWindow.xaml / SettingsWindow.xaml.cs
├── Models/
│   └── AppSettings.cs
├── Services/
│   ├── NetworkService.cs
│   └── SettingsService.cs
├── Helpers/
│   ├── BlurHelper.cs
│   └── WindowHelper.cs
└── icon.ico
```

## Settings Storage

Settings are saved to:
```
%APPDATA%\NetworkMonitor\settings.json
```

## Troubleshooting

### Blur effect not working
- Blur effect requires Windows 10 1803+ or Windows 11
- Try switching to "Solid" background in settings

### Widget not visible
- Check system tray for the icon
- Reset settings by deleting `%APPDATA%\NetworkMonitor\settings.json`

### High memory usage
- Increase refresh rate in settings (e.g., 2000ms)
- Close settings window when not needed

## License

MIT License - Feel free to modify and distribute.

## Contributing

Contributions are welcome! Feel free to submit issues and pull requests.
