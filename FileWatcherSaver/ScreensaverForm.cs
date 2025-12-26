using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FileWatcherSaver
{
    public class ScreensaverForm : Form
    {
        private Panel boxPanel;
        private DataGridView grid;
        private Label titleLabel;
        private Timer animTimer;
        private Point mouseLocation;
        private int speedX = 4, speedY = 4;
        private FileSystemWatcher watcher;
        private BindingList<FileRecord> fileData;

        // Data model
        public class FileRecord {
            public string Time { get; set; }
            public string File { get; set; }
            public string Size { get; set; }
            public string Status { get; set; }
        }

        public ScreensaverForm(Rectangle bounds)
        {
            this.Bounds = bounds;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;

            // --- UI CONSTRUCTION ---
            
            // 1. The Moving Panel
            boxPanel = new Panel();
            boxPanel.Size = new Size(600, 400);
            boxPanel.BackColor = Color.FromArgb(20, 20, 20); // Dark Grey
            boxPanel.BorderStyle = BorderStyle.FixedSingle;
            boxPanel.Location = new Point(100, 100); // Start pos
            this.Controls.Add(boxPanel);

            // 2. The Title
            titleLabel = new Label();
            titleLabel.Text = "ACTIVE FILE TRANSFERS";
            titleLabel.ForeColor = Color.LimeGreen;
            titleLabel.Font = new Font("Consolas", 12, FontStyle.Bold);
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 30;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            boxPanel.Controls.Add(titleLabel);

            // 3. The Grid
            grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.BackgroundColor = Color.Black;
            grid.ForeColor = Color.LimeGreen;
            grid.GridColor = Color.DarkGreen;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.Font = new Font("Consolas", 9);
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.DefaultCellStyle.BackColor = Color.Black;
            grid.DefaultCellStyle.SelectionBackColor = Color.Black; // No highlight
            grid.DefaultCellStyle.SelectionForeColor = Color.LimeGreen;
            
            fileData = new BindingList<FileRecord>();
            grid.DataSource = fileData;
            boxPanel.Controls.Add(grid);

            // --- EVENTS ---
            this.Load += OnLoad;
            this.KeyDown += (s, e) => Application.Exit();
            this.MouseDown += (s, e) => Application.Exit();
            this.MouseMove += OnMouseMove;
            
            animTimer = new Timer();
            animTimer.Interval = 16; // ~60fps
            animTimer.Tick += OnTick;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            Cursor.Hide();
            SetupWatcher();
            animTimer.Start();
        }

        private void SetupWatcher()
        {
            // Simple persistent storage without using Project Properties (which are harder in VS Code)
            string path = "C:\\"; 
            if(File.Exists("config.txt")) path = File.ReadAllText("config.txt");

            titleLabel.Text = $"MONITORING: {path.ToUpper()}";

            watcher = new FileSystemWatcher
            {
                Path = Directory.Exists(path) ? path : "C:\\",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };

            watcher.Changed += (s, e) => AddRecord(e.Name, "MODIFIED");
            watcher.Created += (s, e) => AddRecord(e.Name, "CREATED");
        }

        private void AddRecord(string name, string status)
        {
             // Marshaling back to UI thread
            if (this.InvokeRequired) {
                this.Invoke(new Action(() => AddRecord(name, status)));
                return;
            }

            string size = "UNK";
            try { 
                var info = new FileInfo(Path.Combine(watcher.Path, name));
                size = (info.Length / 1024) + " KB";
            } catch {}

            fileData.Insert(0, new FileRecord { 
                Time = DateTime.Now.ToString("HH:mm:ss"), 
                File = name, 
                Size = size, 
                Status = status 
            });
            
            if (fileData.Count > 25) fileData.RemoveAt(25);
        }

        private void OnTick(object sender, EventArgs e)
        {
            boxPanel.Left += speedX;
            boxPanel.Top += speedY;

            // Bounce X
            if (boxPanel.Left <= 0 || boxPanel.Right >= this.Width) {
                speedX = -speedX;
                boxPanel.Left = Math.Clamp(boxPanel.Left, 0, this.Width - boxPanel.Width);
            }
            // Bounce Y
            if (boxPanel.Top <= 0 || boxPanel.Bottom >= this.Height) {
                speedY = -speedY;
                boxPanel.Top = Math.Clamp(boxPanel.Top, 0, this.Height - boxPanel.Height);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseLocation.IsEmpty)
            {
                if (Math.Abs(mouseLocation.X - e.X) > 10 || Math.Abs(mouseLocation.Y - e.Y) > 10)
                    Application.Exit();
            }
            mouseLocation = e.Location;
        }
    }
}
