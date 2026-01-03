using FileMonitoring;

namespace FileWatcherSaver
{
    public class ScreensaverForm : Form
    {
        private Panel boxPanel;
        private DataGridView grid;
        private Label titleLabel;
        private Label infoLabel;
        private System.Windows.Forms.Timer animTimer;
        private Point mouseLocation;
        private int speedX = 4, speedY = 4;
        private string path = "C:\\";
        private int refreshIntervalTicks = 120; // Refresh every 60 ticks (~1 second)
        private int tickCounter = 0;
        private int maxFilesToShow = 100;

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
            boxPanel.Size = new Size(600, 420);
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
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LastModifiedUtc", HeaderText = "Modified", FillWeight = 10f, MinimumWidth = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "File", FillWeight = 70f, MinimumWidth = 220 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SizeInBytes", HeaderText = "Size (b)", FillWeight = 20f, MinimumWidth = 70 });

            boxPanel.Controls.Add(grid);

            infoLabel = new Label();
            infoLabel.Height = 20;
            infoLabel.Dock = DockStyle.Bottom;
            infoLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoLabel.ForeColor = Color.Gray;
            infoLabel.Font = new Font("Consolas", 8);
            infoLabel.Text = "Move mouse or press any key to exit";
            boxPanel.Controls.Add(infoLabel);

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
            var settings = AppSettings.Load();
            if (settings.DirectoryPath != null) path = settings.DirectoryPath;
            int speed = settings.Speed;
            refreshIntervalTicks = settings.RefreshIntervalTicks;

            if (!Directory.Exists(path)) path = "C:\\";
            
            speedX = speed;
            speedY = speed;

            titleLabel.Text = $"MONITORING: {path.ToUpper()}";
            RefreshFileList();
            animTimer.Start();
        }

        private void RefreshFileList()
        {
            var monitor = new DirectoryMonitor();
            var files = monitor.GetDirectoryListing(path, debugMode: false)
                               .Take(maxFilesToShow)
                               .ToList();

            grid.DataSource = files;
        }

        private void OnTick(object? sender, EventArgs e)
        {
            int newX = boxPanel.Left + speedX;
            int newY = boxPanel.Top + speedY;

            if (tickCounter >= refreshIntervalTicks)
            {
                RefreshFileList();
                tickCounter = 0;
            }
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

            if (refreshIntervalTicks > 120)
            {
                // update info label to show time until next refresh
                int secondsLeft = ((refreshIntervalTicks - tickCounter) / 60) + 1;
                infoLabel.Text = $"Refreshing in {secondsLeft} seconds";
            }

            boxPanel.Location = new Point(newX, newY);
            tickCounter++;
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
