using System;
using System.Drawing;
using System.Windows.Forms;

namespace MeiseiEnterprise
{
    public partial class SplashForm : Form
    {
        private System.Windows.Forms.Timer _timer;
        private int _progress = 0;
        private ProgressBar _progressBar;
        private Label _statusLabel;

        public SplashForm()
        {
            InitializeComponent();
            Program.ApplyClassicTheme(this);
            AuditLogger.LogAction("SPLASH_SCREEN", "Loading corporate resources...");
        }

        private void InitializeComponent()
        {
            this.Text = "Meisei Enterprise Suite";
            this.Size = new Size(500, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.BackColor = Color.FromArgb(212, 208, 200); // Windows 2000 gray

            var titleLabel = new Label
            {
                Text = "MEISEI CORPORATION",
                Font = new Font("Arial", 24, FontStyle.Bold),
                Location = new Point(50, 30),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(0, 0, 128) // Navy blue
            };

            var subtitleLabel = new Label
            {
                Text = "Enterprise Management Suite v1.0.0.0",
                Font = new Font("Arial", 10),
                Location = new Point(50, 75),
                Size = new Size(400, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var copyrightLabel = new Label
            {
                Text = "© 2000 Meisei Corporation. All Rights Reserved.\nUnauthorized access is strictly prohibited and will be prosecuted.",
                Font = new Font("Arial", 8),
                Location = new Point(50, 240),
                Size = new Size(400, 35),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };

            _statusLabel = new Label
            {
                Text = "Initializing corporate systems...",
                Font = new Font("Arial", 9),
                Location = new Point(50, 180),
                Size = new Size(400, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _progressBar = new ProgressBar
            {
                Location = new Point(50, 210),
                Size = new Size(400, 20),
                Style = ProgressBarStyle.Continuous
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(subtitleLabel);
            this.Controls.Add(_statusLabel);
            this.Controls.Add(_progressBar);
            this.Controls.Add(copyrightLabel);

            _timer = new System.Windows.Forms.Timer { Interval = 150 };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _progress += 2;
            _progressBar.Value = Math.Min(_progress, 100);

            string[] statuses = new[]
            {
                "Loading employee database...",
                "Connecting to corporate server...",
                "Verifying user credentials...",
                "Initializing audit systems...",
                "Loading compliance modules...",
                "Establishing secure connection...",
                "Synchronizing with HR database...",
                "Preparing document management system...",
                "Loading inventory database...",
                "Initializing email terminal...",
                "Compiling analytics data...",
                "Preparing user interface...",
                "Ready."
            };

            int statusIndex = _progress / 8;
            if (statusIndex < statuses.Length)
            {
                _statusLabel.Text = statuses[statusIndex];
            }

            if (_progress >= 100)
            {
                _timer.Stop();
                System.Threading.Thread.Sleep(500); // One last annoying delay
                this.Close();
            }
        }
    }
}
