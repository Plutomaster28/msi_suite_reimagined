using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MeiseiEnterprise
{
    public partial class MainForm : Form
    {
        private MenuStrip _menuStrip;
        private StatusStrip _statusStrip;
        private ToolStripStatusLabel _statusLabel;
        private Panel _contentPanel;

        public MainForm()
        {
            InitializeComponent();
            ApplyWindows2000Theme();
            SetupMouseTracking();
            Program.ApplyClassicTheme(this);
            AuditLogger.LogAction("MAIN_FORM_LOADED", "Main application window initialized");
        }

        private void InitializeComponent()
        {
            this.Text = "Meisei Enterprise Suite - [No Document Loaded]";
            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(212, 208, 200);

            // Status bar
            _statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(212, 208, 200)
            };
            _statusLabel = new ToolStripStatusLabel
            {
                Text = $"Ready | User: {Environment.UserName} | Actions Logged: 0"
            };
            _statusStrip.Items.Add(_statusLabel);

            // Content panel
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(212, 208, 200)
            };

            // Welcome label
            var welcomeLabel = new Label
            {
                Text = "MEISEI ENTERPRISE SUITE\n\n" +
                       "Welcome to the Meisei Corporate Management System\n\n" +
                       "Please select a module from the menu above to begin.\n\n" +
                       "All actions are monitored and logged for quality assurance purposes.",
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            _contentPanel.Controls.Add(welcomeLabel);

            CreateMenus();

            this.Controls.Add(_contentPanel);
            this.Controls.Add(_statusStrip);
            this.Controls.Add(_menuStrip);

            // Update status bar periodically
            var statusTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            statusTimer.Tick += (s, e) => UpdateStatusBar();
            statusTimer.Start();

            // Handle form closing
            this.FormClosing += MainForm_FormClosing;
        }

        private void CreateMenus()
        {
            _menuStrip = new MenuStrip
            {
                BackColor = Color.FromArgb(212, 208, 200)
            };

            // File Menu
            var fileMenu = new ToolStripMenuItem("&File");
            fileMenu.DropDownItems.Add(CreateMenuItem("&New", null, File_New));
            fileMenu.DropDownItems.Add(CreateMenuItem("&Open", null, File_Open));
            fileMenu.DropDownItems.Add(CreateMenuItem("&Save", null, File_Save));
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(CreateMenuItem("E&xit", null, File_Exit));

            // Modules Menu
            var modulesMenu = new ToolStripMenuItem("&Modules");
            modulesMenu.DropDownItems.Add(CreateMenuItem("&HR Manager", null, (s, e) => OpenModule("HR Manager")));
            modulesMenu.DropDownItems.Add(CreateMenuItem("&Document Processor", null, (s, e) => OpenModule("Document Processor")));
            modulesMenu.DropDownItems.Add(CreateMenuItem("&Inventory Tracker", null, (s, e) => OpenModule("Inventory Tracker")));
            modulesMenu.DropDownItems.Add(CreateMenuItem("&Email Terminal", null, (s, e) => OpenModule("Email Terminal")));
            modulesMenu.DropDownItems.Add(CreateMenuItem("&Analytics Dashboard", null, (s, e) => OpenModule("Analytics Dashboard")));

            // Tools Menu
            var toolsMenu = new ToolStripMenuItem("&Tools");
            toolsMenu.DropDownItems.Add(CreateMenuItem("&Options", null, Tools_Options));
            toolsMenu.DropDownItems.Add(CreateMenuItem("&Settings", null, Tools_Settings));
            toolsMenu.DropDownItems.Add(CreateMenuItem("&Preferences", null, Tools_Preferences));
            toolsMenu.DropDownItems.Add(new ToolStripSeparator());
            toolsMenu.DropDownItems.Add(CreateMenuItem("&Audit Logs", null, Tools_AuditLogs));

            // Help Menu
            var helpMenu = new ToolStripMenuItem("&Help");
            helpMenu.DropDownItems.Add(CreateMenuItem("&Contents", null, Help_Contents));
            helpMenu.DropDownItems.Add(CreateMenuItem("&Index", null, Help_Index));
            helpMenu.DropDownItems.Add(CreateMenuItem("&Search", null, Help_Search));
            helpMenu.DropDownItems.Add(new ToolStripSeparator());
            helpMenu.DropDownItems.Add(CreateMenuItem("&About", null, Help_About));

            _menuStrip.Items.Add(fileMenu);
            _menuStrip.Items.Add(modulesMenu);
            _menuStrip.Items.Add(toolsMenu);
            _menuStrip.Items.Add(helpMenu);
        }

        private ToolStripMenuItem CreateMenuItem(string text, Image? image, EventHandler handler)
        {
            var item = new ToolStripMenuItem(text, image, handler);
            return item;
        }

        private void ApplyWindows2000Theme()
        {
            // Force classic Windows theme colors
            this.BackColor = Color.FromArgb(212, 208, 200);
            this.Font = new Font("Microsoft Sans Serif", 8.25F);
        }

        private void SetupMouseTracking()
        {
            this.MouseMove += (s, e) => AuditLogger.LogMouseMove(e.X, e.Y);
            this.KeyDown += (s, e) => AuditLogger.LogKeyPress(e.KeyCode);
        }

        private void UpdateStatusBar()
        {
            _statusLabel.Text = $"Ready | User: {Environment.UserName} | Actions Logged: {AuditLogger.GetActionCount()}";
        }

        private void OpenModule(string moduleName)
        {
            AuditLogger.LogClick($"Menu -> Modules -> {moduleName}");
            
            if (!ConfirmationHelper.ConfirmAction($"open the {moduleName} module", 2))
                return;

            ConfirmationHelper.ShowProcessingDialog($"Loading {moduleName}...", 1000);

            Form? moduleForm = moduleName switch
            {
                "HR Manager" => new HRManagerForm(),
                "Document Processor" => new DocumentProcessorForm(),
                "Inventory Tracker" => new InventoryTrackerForm(),
                "Email Terminal" => new EmailTerminalForm(),
                "Analytics Dashboard" => new AnalyticsDashboardForm(),
                _ => null
            };

            if (moduleForm != null)
            {
                // Open as standalone window, not MDI child
                moduleForm.StartPosition = FormStartPosition.CenterScreen;
                moduleForm.Show();
                AuditLogger.LogAction("MODULE_OPENED", moduleName);
            }
        }

        // File Menu Handlers
        private void File_New(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Menu -> File -> New");
            if (ConfirmationHelper.ConfirmAction("create a new file", 4))
            {
                MessageBox.Show("New file functionality requires administrator approval.\n\nPlease contact your system administrator.",
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void File_Open(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Menu -> File -> Open");
            ShowNestedFileDialog();
        }

        private void ShowNestedFileDialog()
        {
            // Nested dialog hell
            if (MessageBox.Show("Do you want to open a file?", "Step 1/7", MessageBoxButtons.YesNo) == DialogResult.No) return;
            if (MessageBox.Show("Are you sure?", "Step 2/7", MessageBoxButtons.YesNo) == DialogResult.No) return;
            if (MessageBox.Show("Select file type:\n\nClick Yes for Documents\nClick No to cancel", "Step 3/7", MessageBoxButtons.YesNo) == DialogResult.No) return;
            if (MessageBox.Show("Select location:\n\nClick Yes for Local Files\nClick No to cancel", "Step 4/7", MessageBoxButtons.YesNo) == DialogResult.No) return;
            if (MessageBox.Show("Verify security clearance:\n\nDo you have permission?", "Step 5/7", MessageBoxButtons.YesNo) == DialogResult.No) return;
            
            ConfirmationHelper.ShowProcessingDialog("Scanning for files...", 1500);
            
            if (MessageBox.Show("No files found matching your criteria.\n\nWould you like to try again?", "Step 6/7", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MessageBox.Show("Please contact your system administrator to configure file access permissions.", "Step 7/7", MessageBoxButtons.OK);
            }
        }

        private void File_Save(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Menu -> File -> Save");
            if (ConfirmationHelper.ConfirmAction("save the current document", 5))
            {
                ConfirmationHelper.ShowProcessingDialog("Encrypting document...", 1000);
                ConfirmationHelper.ShowProcessingDialog("Contacting server...", 1200);
                ConfirmationHelper.ShowProcessingDialog("Verifying integrity...", 900);
                MessageBox.Show("Document saved successfully.\n\nBackup created on server: \\\\CORPORATE\\BACKUP\\DOCS\\2000\\10\\01\\",
                    "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void File_Exit(object? sender, EventArgs e)
        {
            this.Close();
        }

        // Tools Menu Handlers
        private void Tools_Options(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Menu -> Tools -> Options");
            MessageBox.Show("This section requires:\n\n• Manager approval\n• HR clearance\n• IT department authorization\n\nPlease submit form HR-2847B for access.",
                "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void Tools_Settings(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Menu -> Tools -> Settings");
            MessageBox.Show("Settings have been disabled by your system administrator.",
                "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void Tools_Preferences(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Menu -> Tools -> Preferences");
            MessageBox.Show("Your preferences are managed centrally.\n\nNo user modifications allowed.",
                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Tools_AuditLogs(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Menu -> Tools -> Audit Logs");
            MessageBox.Show($"Current session activity:\n\nTotal actions logged: {AuditLogger.GetActionCount()}\nLog file: {AuditLogger.GetLogPath()}\n\nThis data is synchronized with corporate servers every 5 minutes.",
                "Audit Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Help Menu Handlers
        private void Help_Contents(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Menu -> Help -> Contents");
            MessageBox.Show("Help files are stored on the corporate server.\n\nServer is currently offline.\n\nPlease try again later or contact IT support at extension 5555.",
                "Help Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void Help_Index(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Menu -> Help -> Index");
            MessageBox.Show("Index is being rebuilt.\n\nEstimated time remaining: 47 minutes",
                "Please Wait", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Help_Search(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Menu -> Help -> Search");
            ConfirmationHelper.ShowProcessingDialog("Searching help database...", 2000);
            MessageBox.Show("Search returned 0 results.",
                "Search Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Help_About(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Menu -> Help -> About");
            MessageBox.Show("Meisei Enterprise Suite\nVersion 1.0.0.0\n\n© 2000 Meisei Corporation\n\nLicensed to: " + Environment.UserName + "\nMachine ID: " + Environment.MachineName + "\n\nAll rights reserved.\nPatent pending.\n\nThis software is monitored for compliance purposes.",
                "About Meisei Enterprise", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            AuditLogger.LogAction("FORM_CLOSING", "User attempting to close application");
            
            if (!ConfirmationHelper.ConfirmAction("exit the application", 4))
            {
                e.Cancel = true;
                return;
            }

            ConfirmationHelper.ShowProcessingDialog("Saving session data...", 800);
            ConfirmationHelper.ShowProcessingDialog("Uploading logs to corporate server...", 1000);
            ConfirmationHelper.ShowProcessingDialog("Closing connections...", 600);

            AuditLogger.Close();

            MessageBox.Show("Thank you for using Meisei Enterprise Suite.\n\nYour session data has been archived.\n\nHave a productive day!",
                "Goodbye", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
