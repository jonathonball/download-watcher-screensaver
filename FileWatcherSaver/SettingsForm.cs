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
            this.Size = new Size(400, 150);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lbl = new Label { Text = "Folder to Monitor:", Location = new Point(10, 15), AutoSize = true };
            this.Controls.Add(lbl);

            txtPath = new TextBox { Location = new Point(10, 40), Width = 280 };
            if(File.Exists("config.txt")) txtPath.Text = File.ReadAllText("config.txt");
            else txtPath.Text = "C:\\";
            this.Controls.Add(txtPath);

            Button btnSave = new Button { Text = "Save", Location = new Point(300, 38) };
            btnSave.Click += (s, e) => {
                File.WriteAllText("config.txt", txtPath.Text);
                MessageBox.Show("Saved!");
                this.Close();
            };
            this.Controls.Add(btnSave);
        }
    }
}
