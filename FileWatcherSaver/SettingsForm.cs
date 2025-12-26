using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace FileWatcherSaver
{
    public class SettingsForm : Form
    {
        private TextBox txtPath;
        
        public SettingsForm()
        {
            this.Text = "Screensaver Settings";
            this.Size = new Size(400, 280);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lbl = new Label { Text = "Folder to Monitor:", Location = new Point(10, 15), AutoSize = true };
            this.Controls.Add(lbl);

            txtPath = new TextBox { Location = new Point(10, 40), Width = 280 };
            this.Controls.Add(txtPath);

            Label lblSpeed = new Label { Text = "Movement Speed:", Location = new Point(10, 80), AutoSize = true };
            this.Controls.Add(lblSpeed);

            TrackBar trackSpeed = new TrackBar { Location = new Point(10, 100), Width = 280, Minimum = 1, Maximum = 20, TickFrequency = 1 };
            this.Controls.Add(trackSpeed);

            Label lblSpeedVal = new Label { Location = new Point(300, 105), AutoSize = true };
            this.Controls.Add(lblSpeedVal);

            trackSpeed.Scroll += (s, e) => lblSpeedVal.Text = trackSpeed.Value.ToString();

            var settings = AppSettings.Load();
            txtPath.Text = settings.DirectoryPath;
            trackSpeed.Value = settings.Speed;
            lblSpeedVal.Text = trackSpeed.Value.ToString();

            Button btnSave = new Button { Text = "Save", Location = new Point(300, 140), Height = 30 };
            btnSave.Click += (s, e) => {
                var newSettings = new AppSettings { 
                    DirectoryPath = txtPath.Text, 
                    Speed = trackSpeed.Value 
                };
                newSettings.Save();
                MessageBox.Show("Saved!");
                this.Close();
            };
            this.Controls.Add(btnSave);

            Label lblConfig = new Label { Text = "Settings File Location:", Location = new Point(10, 180), AutoSize = true };
            this.Controls.Add(lblConfig);

            TextBox txtConfigPath = new TextBox { 
                Text = AppSettings.SettingsFile, 
                Location = new Point(10, 200), 
                Width = 360, 
                ReadOnly = true 
            };
            this.Controls.Add(txtConfigPath);
        }
    }

    public class AppSettings
    {
        public string DirectoryPath { get; set; } = @"C:\";
        public int Speed { get; set; } = 4;

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
