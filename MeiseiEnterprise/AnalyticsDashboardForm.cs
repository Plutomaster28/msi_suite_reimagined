using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MeiseiEnterprise
{
    public partial class AnalyticsDashboardForm : Form
    {
        private Panel _chartPanel;
        private ComboBox _chartSelector;
        private Dictionary<string, string> _charts;

        public AnalyticsDashboardForm()
        {
            InitializeComponent();
            Program.ApplyClassicTheme(this);
            LoadCharts();
            AuditLogger.LogAction("ANALYTICS_DASHBOARD_OPENED", "Analytics Dashboard module initialized");
        }

        private void InitializeComponent()
        {
            this.Text = "Meisei Analytics Dashboard - Corporate Metrics";
            this.Size = new Size(1000, 700);
            this.BackColor = Color.FromArgb(212, 208, 200);

            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(212, 208, 200)
            };

            var label = new Label
            {
                Text = "Select Chart to View (Each view change requires 3 confirmations):",
                Location = new Point(20, 15),
                Size = new Size(500, 20),
                Font = new Font("Arial", 10)
            };

            _chartSelector = new ComboBox
            {
                Location = new Point(20, 40),
                Size = new Size(400, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.System
            };
            _chartSelector.SelectedIndexChanged += ChartSelector_SelectedIndexChanged;

            var refreshButton = new Button
            {
                Text = "Refresh Data",
                Location = new Point(430, 38),
                Size = new Size(120, 28),
                FlatStyle = FlatStyle.System
            };
            refreshButton.Click += (s, e) => RefreshData();

            topPanel.Controls.Add(label);
            topPanel.Controls.Add(_chartSelector);
            topPanel.Controls.Add(refreshButton);

            _chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AutoScroll = true
            };

            this.Controls.Add(_chartPanel);
            this.Controls.Add(topPanel);
        }

        private void LoadCharts()
        {
            _charts = new Dictionary<string, string>
            {
                { "Employee Coffee Consumption", "Average cups per day: 4.7\nPeak time: 9:30 AM\nTotal weekly consumption: 235 cups" },
                { "Mouse Clicks Per Hour", "Average: 847 clicks/hour\nHighest: John Smith (1,203 clicks/hour)\nLowest: Jane Doe (412 clicks/hour)" },
                { "Keyboard Utilization", "Total keystrokes today: 125,847\nMost used key: E (12,394 times)\nLeast used key: Z (47 times)" },
                { "Email Response Time", "Average response: 2.4 hours\nFastest: 12 minutes\nSlowest: 3.2 days" },
                { "Meeting Duration Analysis", "Average meeting: 47 minutes\nTotal meetings this week: 87\nTime spent in meetings: 68.2 hours" },
                { "Printer Usage Statistics", "Pages printed today: 1,247\nColor pages: 23%\nPaper jams: 8" },
                { "Bathroom Break Frequency", "Average breaks per day: 5.3\nAverage duration: 4.2 minutes\nPeak time: 10:45 AM" },
                { "Elevator Wait Time", "Average wait: 2.7 minutes\nLongest wait: 8.3 minutes\nTotal elevator trips: 1,034" },
                { "Desk Chair Adjustments", "Average adjustments per day: 12.4\nHeight changes: 73%\nBackrest angle: 27%" },
                { "Stapler Click Frequency", "Total staples used: 347\nAverage per document: 2.1\nMisfires: 23" }
            };

            foreach (var chart in _charts.Keys)
            {
                _chartSelector.Items.Add(chart);
            }

            if (_chartSelector.Items.Count > 0)
            {
                _chartSelector.SelectedIndex = 0;
            }
        }

        private void ChartSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_chartSelector.SelectedItem == null) return;

            AuditLogger.LogClick("Chart Selection: " + _chartSelector.SelectedItem.ToString());

            if (!ConfirmationHelper.ConfirmAction("view the '" + _chartSelector.SelectedItem.ToString() + "' chart", 3))
            {
                return;
            }

            ShowProgressDialog("Loading chart data...", 2000);
            DisplayChart(_chartSelector.SelectedItem.ToString());
        }

        private void DisplayChart(string chartName)
        {
            _chartPanel.Controls.Clear();

            var titleLabel = new Label
            {
                Text = chartName.ToUpper(),
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(900, 30),
                BackColor = Color.White
            };

            var dataLabel = new Label
            {
                Text = _charts[chartName],
                Font = new Font("Courier New", 11),
                Location = new Point(20, 70),
                Size = new Size(900, 150),
                BackColor = Color.White
            };

            var chartImage = DrawSimpleChart(chartName);
            var pictureBox = new PictureBox
            {
                Image = chartImage,
                Location = new Point(20, 240),
                Size = chartImage.Size,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var footerLabel = new Label
            {
                Text = "Data as of: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\nNext update: In 24 hours\nReport generated by: Meisei Analytics Engine v2.0",
                Font = new Font("Arial", 9),
                Location = new Point(20, 240 + chartImage.Height + 20),
                Size = new Size(900, 80),
                BackColor = Color.White,
                ForeColor = Color.Gray
            };

            _chartPanel.Controls.Add(titleLabel);
            _chartPanel.Controls.Add(dataLabel);
            _chartPanel.Controls.Add(pictureBox);
            _chartPanel.Controls.Add(footerLabel);

            AuditLogger.LogAction("CHART_DISPLAYED", "Displayed chart: " + chartName);
        }

        private Bitmap DrawSimpleChart(string chartName)
        {
            var bitmap = new Bitmap(700, 400);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);

                var pen = new Pen(Color.Black, 2);
                g.DrawLine(pen, 50, 350, 650, 350);
                g.DrawLine(pen, 50, 50, 50, 350);

                var random = new Random(chartName.GetHashCode());
                var barWidth = 80;
                var spacing = 100;
                
                for (int i = 0; i < 6; i++)
                {
                    var height = random.Next(50, 280);
                    var x = 70 + (i * spacing);
                    var y = 350 - height;

                    var brush = new SolidBrush(Color.FromArgb(0, 0, 128));
                    g.FillRectangle(brush, x, y, barWidth, height);
                    g.DrawRectangle(Pens.Black, x, y, barWidth, height);

                    var value = (height * 100 / 280).ToString();
                    var font = new Font("Arial", 9);
                    var textSize = g.MeasureString(value, font);
                    g.DrawString(value, font, Brushes.Black, x + barWidth/2 - textSize.Width/2, y - 20);

                    g.DrawString("Day " + (i+1).ToString(), font, Brushes.Black, x + 10, 355);
                }

                var titleFont = new Font("Arial", 12, FontStyle.Bold);
                g.DrawString(chartName, titleFont, Brushes.Black, 200, 10);

                var gridPen = new Pen(Color.LightGray, 1);
                for (int i = 1; i < 6; i++)
                {
                    var y = 50 + (i * 50);
                    g.DrawLine(gridPen, 50, y, 650, y);
                }
            }

            return bitmap;
        }

        private void RefreshData()
        {
            AuditLogger.LogClick("Refresh Data Button");

            if (!ConfirmationHelper.ConfirmAction("refresh the dashboard data", 2))
                return;

            ShowProgressDialog("Recalculating metrics...", 3000);

            MessageBox.Show("Data refresh complete.\n\nAll metrics have been recalculated.\nLog entries: 1,247 new records processed.",
                "Refresh Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowProgressDialog(string message, int durationMs)
        {
            var progressForm = new Form
            {
                Text = "Please Wait",
                Size = new Size(400, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(212, 208, 200)
            };

            var label = new Label
            {
                Text = message,
                Font = new Font("Arial", 10),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 10, 0, 0)
            };

            var progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Location = new Point(50, 60),
                Size = new Size(300, 25)
            };

            progressForm.Controls.Add(label);
            progressForm.Controls.Add(progressBar);

            var timer = new System.Windows.Forms.Timer { Interval = durationMs };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                progressForm.Close();
            };
            timer.Start();

            progressForm.ShowDialog();
        }
    }
}
