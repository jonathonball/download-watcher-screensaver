namespace FileWatcherSaver
{
    public class SettingsForm : Form
    {

        public SettingsForm()
        {
            this.Text = "Screensaver Settings";
            this.Size = new Size(450, 380);
            this.StartPosition = FormStartPosition.CenterScreen;

            int leftOffset = 10;

            Label lbl = new Label {
                Text = "Folder to Monitor:",
                Location = new Point(leftOffset, 15),
                AutoSize = true
            };
            this.Controls.Add(lbl);

            TextBox txtPath = new TextBox {
                Location = new Point(leftOffset, 40),
                Width = 280
            };
            this.Controls.Add(txtPath);

            Label lblSpeed = new Label {
                Text = "Movement Speed:",
                Location = new Point(leftOffset, 80),
                AutoSize = true
            };
            this.Controls.Add(lblSpeed);

            TrackBar trackSpeed = new TrackBar {
                Location = new Point(leftOffset, 100),
                Width = 280,
                Minimum = 1,
                Maximum = 6,
                TickFrequency = 1
            };
            this.Controls.Add(trackSpeed);

            Label lblSpeedVal = new Label {
                Location = new Point(300, 105),
                AutoSize = true
            };
            this.Controls.Add(lblSpeedVal);

            trackSpeed.Scroll += (s, e) => lblSpeedVal.Text = trackSpeed.Value.ToString();

            Label lblRefresh = new Label {
                Text = "Refresh Interval (seconds):",
                Location = new Point(leftOffset, 155),
                AutoSize = true
            };
            this.Controls.Add(lblRefresh);

            TextBox textRefresh = new TextBox {
                Location = new Point(leftOffset, 175),
                Width = 50
            };
            this.Controls.Add(textRefresh);

            textRefresh.LostFocus += (s, e) =>
            {
                if (int.TryParse(textRefresh.Text, out int val))
                {
                    if (val < 1) val = 1;
                    if (val > 3600) val = 3600;
                    textRefresh.Text = val.ToString();
                }
                else
                {
                    textRefresh.Text = "120";
                }
            };

            Label lblConfig = new Label {
                Text = "Settings path:",
                Location = new Point(10, 230),
                AutoSize = true
            };
            this.Controls.Add(lblConfig);

            TextBox txtConfigPath = new TextBox {
                Text = AppSettings.SettingsFile,
                Location = new Point(10, 250),
                Width = 410,
                ReadOnly = true
            };
            this.Controls.Add(txtConfigPath);

            var settings = AppSettings.Load();
            txtPath.Text = settings.getDirectoryPathOrDefault();
            trackSpeed.Value = settings.Speed;
            lblSpeedVal.Text = trackSpeed.Value.ToString();
            textRefresh.Text = (settings.RefreshIntervalTicks / 60).ToString();

            Button btnSave = new Button {
                Text = "Save",
                Location = new Point(340, 295),
                Height = 30
            };
            btnSave.Click += (s, e) => {
                if (!Directory.Exists(txtPath.Text)) txtPath.Text = "%USERPROFILE%\\Downloads";
                var newSettings = new AppSettings { 
                    DirectoryPath = txtPath.Text,
                    Speed = trackSpeed.Value,
                    RefreshIntervalTicks = int.Parse(textRefresh.Text) * 60
                };
                newSettings.Save();
                MessageBox.Show("Saved!");
                this.Close();
            };
            this.Controls.Add(btnSave);

        }
    }

}
