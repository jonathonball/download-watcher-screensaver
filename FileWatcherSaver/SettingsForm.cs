using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FileWatcherSaver
{
    public class SettingsForm : Form
    {
        private TextBox txtPath;
        
        public SettingsForm()
        {
            this.Text = "Screensaver Settings";
            this.Size = new Size(400, 220);
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

            if (File.Exists("config.txt")) {
                var lines = File.ReadAllLines("config.txt");
                if (lines.Length > 0) txtPath.Text = lines[0];
                if (lines.Length > 1 && int.TryParse(lines[1], out int s)) trackSpeed.Value = s;
                else trackSpeed.Value = 4;
            } else {
                txtPath.Text = "C:\\";
                trackSpeed.Value = 4;
            }
            lblSpeedVal.Text = trackSpeed.Value.ToString();

            Button btnSave = new Button { Text = "Save", Location = new Point(300, 140) };
            btnSave.Click += (s, e) => {
                File.WriteAllLines("config.txt", new string[] { txtPath.Text, trackSpeed.Value.ToString() });
                MessageBox.Show("Saved!");
                this.Close();
            };
            this.Controls.Add(btnSave);
        }
    }
}
