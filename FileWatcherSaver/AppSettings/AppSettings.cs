using System.Text.Json;

namespace FileWatcherSaver
{
    public class AppSettings
    {
        public string DirectoryPath { get; set; } = @"C:\";
        public int Speed { get; set; } = 4;
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
    }
}

