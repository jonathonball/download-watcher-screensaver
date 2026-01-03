using System.Text.Json;

namespace FileWatcherSaver
{
    public class AppSettings
    {
        public string? DirectoryPath { get; set; } = null;
        public int Speed { get; set; } = 1;
        public int RefreshIntervalTicks { get; set; } = 120;

        public static string SettingsFile => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FileWatcherSaver",
            "settings.json");

        public static AppSettings Load()
        {
            try {
                if (File.Exists(SettingsFile))
                    return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsFile)) ?? new AppSettings();
            } catch { }
            return new AppSettings();
        }

        public void Save()
        {
            string? dir = Path.GetDirectoryName(SettingsFile);
            if (dir != null && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(this));
        }

        public string getDirectoryPathOrDefault() {
            // Choose the configured directory, but prefer the user's Downloads folder when the setting is not provided
            string? selectedPath = DirectoryPath;
            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                selectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            }

            // Expand environment variables (support values like %USERPROFILE%\\Downloads)
            selectedPath = Environment.ExpandEnvironmentVariables(selectedPath);

            // If the selected path doesn't exist, fall back to C:\
            if (!Directory.Exists(selectedPath))
            {
                selectedPath = "C:\\";
            }

            return selectedPath;
        }
    }
}

