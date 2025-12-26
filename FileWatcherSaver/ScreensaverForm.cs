using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FileWatcherSaver
{
    public class ScreensaverForm : Form
    {
        private Panel boxPanel;
        private DataGridView grid;
        private Label titleLabel;
        private System.Windows.Forms.Timer animTimer;
        private Point mouseLocation;
        private int speedX = 4, speedY = 4;
        private FileSystemWatcher? watcher;
        private BindingList<FileRecord> fileData;

        // Data model
        public class FileRecord {
            public string? Time { get; set; }
            public string? File { get; set; }
            public string? Size { get; set; }
        }

        public ScreensaverForm(Rectangle bounds)
        {
            this.Bounds = bounds;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.DoubleBuffered = true;

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
            titleLabel.Text = "MONITORING: ";
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
            grid.DefaultCellStyle.ForeColor = Color.LimeGreen;
            grid.DefaultCellStyle.SelectionBackColor = Color.Black; // No highlight
            grid.DefaultCellStyle.SelectionForeColor = Color.LimeGreen;

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.LimeGreen;
            
            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Time", HeaderText = "Time", FillWeight = 10f, MinimumWidth = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "File", HeaderText = "File", FillWeight = 70f, MinimumWidth = 220 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Size", HeaderText = "Size", FillWeight = 20f, MinimumWidth = 70 });

            fileData = new BindingList<FileRecord>();
            grid.DataSource = fileData;
            boxPanel.Controls.Add(grid);

            // --- EVENTS ---
            this.Load += OnLoad;
            this.KeyDown += (s, e) => Application.Exit();
            this.MouseDown += (s, e) => Application.Exit();
            this.MouseMove += OnMouseMove;
            
            animTimer = new System.Windows.Forms.Timer();
            animTimer.Interval = 16; // ~60fps
            animTimer.Tick += OnTick;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            Cursor.Hide();
            SetupWatcher();
            animTimer.Start();
        }

        private void SetupWatcher()
        {
            var settings = AppSettings.Load();
            string path = settings.DirectoryPath;
            int speed = settings.Speed;

            if (!Directory.Exists(path)) path = "C:\\";
            
            speedX = speed;
            speedY = speed;

            titleLabel.Text = $"MONITORING: {path.ToUpper()}";

            try
            {
                var dirInfo = new DirectoryInfo(path);
                var files = dirInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).Take(25);
                foreach (var f in files)
                {
                    fileData.Add(new FileRecord {
                        Time = f.LastWriteTime.ToString("HH:mm:ss"),
                        File = f.Name,
                        Size = FormatBytes(f.Length)
                    });
                }
            }
            catch 
            {
                fileData.Add(new FileRecord { 
                    Time = DateTime.Now.ToString("HH:mm:ss"), 
                    File = "ERROR: Check Permissions", 
                    Size = "0 KB" 
                });
            }

            watcher = new FileSystemWatcher
            {
                Path = path,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };

            watcher.Changed += (s, e) => AddRecord(e.Name ?? "Unknown", "MODIFIED");
            watcher.Created += (s, e) => AddRecord(e.Name ?? "Unknown", "CREATED");
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
                var info = new FileInfo(Path.Combine(watcher?.Path ?? "", name));
                size = FormatBytes(info.Length);
            } catch {}

            fileData.Insert(0, new FileRecord { 
                Time = DateTime.Now.ToString("HH:mm:ss"), 
                File = name, 
                Size = size
            });
            
            if (fileData.Count > 25) fileData.RemoveAt(25);
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes < 0) return "0 B";
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }

        private void OnTick(object? sender, EventArgs e)
        {
            int newX = boxPanel.Left + speedX;
            int newY = boxPanel.Top + speedY;

            // Bounce X
            if (newX <= 0 || newX + boxPanel.Width >= this.Width) {
                speedX = -speedX;
                newX = Math.Clamp(newX, 0, this.Width - boxPanel.Width);
            }
            // Bounce Y
            if (newY <= 0 || newY + boxPanel.Height >= this.Height) {
                speedY = -speedY;
                newY = Math.Clamp(newY, 0, this.Height - boxPanel.Height);
            }

            boxPanel.Location = new Point(newX, newY);
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
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
