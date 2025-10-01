using System;
using System.Drawing;
using System.Windows.Forms;

namespace MeiseiEnterprise
{
    public partial class DocumentProcessorForm : Form
    {
        private RichTextBox _textBox;
        private ToolStrip _toolbar;
        private System.Windows.Forms.Timer _autoSaveTimer;
        private int _wordCount = 0;

        public DocumentProcessorForm()
        {
            InitializeComponent();
            Program.ApplyClassicTheme(this);
            SetupAutoSave();
            AuditLogger.LogAction("DOCUMENT_PROCESSOR_OPENED", "Document Processor module initialized");
        }

        private void InitializeComponent()
        {
            this.Text = "Meisei Document Processor - [Untitled Document]";
            this.Size = new Size(800, 600);
            this.BackColor = Color.FromArgb(212, 208, 200);

            CreateToolbar();

            _textBox = new RichTextBox
            {
                Location = new Point(10, 40),
                Size = new Size(760, 500),
                Font = new Font("Courier New", 10F),
                BackColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };
            _textBox.TextChanged += TextBox_TextChanged;
            _textBox.KeyPress += (s, e) => AuditLogger.LogKeyPress((Keys)e.KeyChar);

            var statusBar = new StatusStrip
            {
                BackColor = Color.FromArgb(212, 208, 200)
            };
            var statusLabel = new ToolStripStatusLabel("Words: 0 | Auto-save enabled");
            statusBar.Items.Add(statusLabel);

            _textBox.TextChanged += (s, e) =>
            {
                _wordCount = _textBox.Text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
                statusLabel.Text = $"Words: {_wordCount} | Characters: {_textBox.Text.Length} | Auto-save enabled";
            };

            this.Controls.Add(_textBox);
            this.Controls.Add(_toolbar);
            this.Controls.Add(statusBar);
        }

        private void CreateToolbar()
        {
            _toolbar = new ToolStrip
            {
                BackColor = Color.FromArgb(212, 208, 200),
                GripStyle = ToolStripGripStyle.Hidden
            };

            var boldButton = new ToolStripButton("Bold");
            boldButton.Click += (s, e) => ApplyFormatting("bold");

            var italicButton = new ToolStripButton("Italic");
            italicButton.Click += (s, e) => ApplyFormatting("italic");

            var underlineButton = new ToolStripButton("Underline");
            underlineButton.Click += (s, e) => ApplyFormatting("underline");

            _toolbar.Items.Add(boldButton);
            _toolbar.Items.Add(italicButton);
            _toolbar.Items.Add(underlineButton);
            _toolbar.Items.Add(new ToolStripSeparator());

            var printButton = new ToolStripButton("Print");
            printButton.Click += Print_Click;
            _toolbar.Items.Add(printButton);

            var saveButton = new ToolStripButton("Save");
            saveButton.Click += Save_Click;
            _toolbar.Items.Add(saveButton);
        }

        private void ApplyFormatting(string format)
        {
            AuditLogger.LogClick($"Format Button: {format}");
            
            if (!ConfirmationHelper.ConfirmAction($"apply {format} formatting", 2))
                return;

            if (_textBox.SelectionLength == 0)
            {
                MessageBox.Show("Please select text first.\n\nFormatting requires text selection.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ConfirmationHelper.ShowProcessingDialog($"Applying {format} formatting...", 800);

            var currentFont = _textBox.SelectionFont ?? _textBox.Font;
            FontStyle newStyle = currentFont.Style;

            switch (format.ToLower())
            {
                case "bold":
                    newStyle = newStyle ^ FontStyle.Bold;
                    break;
                case "italic":
                    newStyle = newStyle ^ FontStyle.Italic;
                    break;
                case "underline":
                    newStyle = newStyle ^ FontStyle.Underline;
                    break;
            }

            _textBox.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newStyle);
            
            MessageBox.Show($"Formatting applied.\n\nThis change has been logged.",
                "Format Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Print_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Print Button");
            
            // Step 1
            if (MessageBox.Show("Do you want to print this document?", "Print - Step 1 of 5",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            // Step 2
            if (MessageBox.Show("Select printer:\n\nYes = Network Printer\nNo = Cancel",
                "Print - Step 2 of 5", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            // Step 3
            ConfirmationHelper.ShowProcessingDialog("Connecting to print server...", 1500);

            // Step 4
            if (MessageBox.Show("Select print quality:\n\nYes = Draft Mode\nNo = Cancel\n\n(High quality requires manager approval)",
                "Print - Step 3 of 5", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            // Step 5
            if (MessageBox.Show("Number of copies: 1\n\nIs this correct?", "Print - Step 4 of 5",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            // Step 6
            ConfirmationHelper.ShowProcessingDialog("Spooling print job...", 1200);
            ConfirmationHelper.ShowProcessingDialog("Sending to printer...", 1000);

            // Final step
            MessageBox.Show("Print job failed.\n\nError: Printer offline\n\nPlease contact IT support.\n\nTicket #2847 has been created.",
                "Print - Step 5 of 5", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Save_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Save Button");
            
            if (!ConfirmationHelper.ConfirmAction("save this document", 4))
                return;

            PerformSave();
        }

        private void SetupAutoSave()
        {
            _autoSaveTimer = new System.Windows.Forms.Timer
            {
                Interval = 30000 // 30 seconds
            };
            _autoSaveTimer.Tick += AutoSave_Tick;
            _autoSaveTimer.Start();
        }

        private void AutoSave_Tick(object? sender, EventArgs e)
        {
            if (_textBox.Text.Length > 0)
            {
                AuditLogger.LogAction("AUTO_SAVE", "Automatic save triggered");
                
                // Show the annoying auto-save dialog
                var saveForm = new Form
                {
                    Text = "Auto-Saving",
                    Size = new Size(400, 150),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterScreen,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    ControlBox = false,
                    TopMost = true,
                    BackColor = Color.FromArgb(212, 208, 200)
                };

                var label = new Label
                {
                    Text = "Auto-saving document...\n\nPlease wait, do not close the application.",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font("Microsoft Sans Serif", 10F)
                };

                var progressBar = new ProgressBar
                {
                    Style = ProgressBarStyle.Marquee,
                    MarqueeAnimationSpeed = 30,
                    Dock = DockStyle.Bottom,
                    Height = 23
                };

                saveForm.Controls.Add(label);
                saveForm.Controls.Add(progressBar);

                var timer = new System.Windows.Forms.Timer { Interval = 2000 };
                timer.Tick += (s, ev) => { timer.Stop(); saveForm.Close(); };
                timer.Start();

                saveForm.ShowDialog();
            }
        }

        private void PerformSave()
        {
            ConfirmationHelper.ShowProcessingDialog("Validating document...", 800);
            ConfirmationHelper.ShowProcessingDialog("Checking permissions...", 900);
            ConfirmationHelper.ShowProcessingDialog("Encrypting content...", 1000);
            ConfirmationHelper.ShowProcessingDialog("Contacting file server...", 1100);
            ConfirmationHelper.ShowProcessingDialog("Writing to disk...", 700);
            ConfirmationHelper.ShowProcessingDialog("Creating backup...", 900);
            ConfirmationHelper.ShowProcessingDialog("Updating metadata...", 600);

            MessageBox.Show($"Document saved successfully!\n\n" +
                           $"Location: C:\\Corporate\\Documents\\{DateTime.Now:yyyyMMdd}\\untitled.doc\n" +
                           $"Size: {_textBox.Text.Length} bytes\n" +
                           $"Words: {_wordCount}\n\n" +
                           $"Backup created: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TextBox_TextChanged(object? sender, EventArgs e)
        {
            // Log every 100 characters
            if (_textBox.Text.Length % 100 == 0 && _textBox.Text.Length > 0)
            {
                AuditLogger.LogAction("TEXT_CHANGED", $"Document length: {_textBox.Text.Length} characters");
            }
        }
    }
}
