namespace FileWatcherSaver
{
    public class SettingsForm : Form
    {
        private TextBox txtPath;
        
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

            txtPath = new TextBox {
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
                Maximum = 20,
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
                Text = "Refresh Interval (ticks):",
                Location = new Point(leftOffset, 155),
                AutoSize = true
            };
            this.Controls.Add(lblRefresh);

            TrackBar trackRefresh = new TrackBar {
                Location = new Point(leftOffset, 175),
                Width = 280,
                Minimum = 30,
                Maximum = 5000,
                TickFrequency = 1
            };
            this.Controls.Add(trackRefresh);

            Label lblRefreshVal = new Label {
                Location = new Point(300, 180),
                AutoSize = true
            };
            this.Controls.Add(lblRefreshVal);

            trackRefresh.Scroll += (s, e) => lblRefreshVal.Text = trackRefresh.Value.ToString();


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
            txtPath.Text = settings.DirectoryPath;
            trackSpeed.Value = settings.Speed;
            lblSpeedVal.Text = trackSpeed.Value.ToString();
            trackRefresh.Value = settings.RefreshIntervalTicks;
            lblRefreshVal.Text = trackRefresh.Value.ToString();

            Button btnSave = new Button {
                Text = "Save",
                Location = new Point(340, 295),
                Height = 30
            };
            btnSave.Click += (s, e) => {
                var newSettings = new AppSettings { 
                    DirectoryPath = txtPath.Text, 
                    Speed = trackSpeed.Value, 
                    RefreshIntervalTicks = trackRefresh.Value
                };
                newSettings.Save();
                MessageBox.Show("Saved!");
                this.Close();
            };
            this.Controls.Add(btnSave);

        }
    }

}
