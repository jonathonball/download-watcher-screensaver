# download-watcher-screensaver
This is an OLED burn-in prevention tool. This is a Windows screensaver that watches a folder. It is designed for keeping an eye on long running file transfers on OLED. The information panel will move around the screen in a path similar to an old school DVD screensaver. Should run on anything that supports DotNet SDK 8.

## Features
- Shows up to 15 of the newest files in a directory with the newest timestamps at the top
- Displays file timestamp, filename, and size
- Moves the data around the screen to prevent burn-in
- Configurable directory path to monitor
- Configurable refresh interval for file metadata refresh
  - Default is 120 ticks or once every 2 seconds
  - Adjustable between 30 ticks (twice per second) and 3600 ticks (once every 60 seconds)
- Configurable refresh interval for window movement on screen
- Settings stored as json text in `%AppData%\Local\FileWatcherSaver`

## Prerequisites
To build this project, you need the .NET 8 SDK.

**Install via Winget:**
```powershell
winget install Microsoft.DotNet.SDK.8
```

## Building
1. Open a terminal in the project directory.
2. Run the publish command to create a single-file executable:

```powershell
cd FileWatcherSaver
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true -o ../publish
```
*Note: Change `--self-contained false` to `true` if you want to bundle the .NET runtime inside the screensaver (larger file size, but no separate .NET installation required on the target machine).*

## Installation
1. Navigate to the `publish` folder created in the previous step.
2. Locate `FileWatcherSaver.exe`.
3. Rename the file extension from `.exe` to `.scr` (e.g., `FileWatcherSaver.scr`).
4. Right-click `FileWatcherSaver.scr` and select **Install**.
   - This will open the Windows Screen Saver Settings dialog with this screensaver selected.
5. Click **Settings...** to configure the folder to monitor and animation speed.
6. Click **Apply** or **OK** to set it as your active screensaver.

## Example
![Example](example.png)

## Note
Read the LICENSE file for more information.
