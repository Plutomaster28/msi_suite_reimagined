using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MeiseiEnterprise
{
    public partial class EmailTerminalForm : Form
    {
        private ListBox _folderList;
        private ListView _emailList;
        private RichTextBox _emailContent;
        private List<Email> _emails;
        private bool _isLoggedIn = false;

        public EmailTerminalForm()
        {
            InitializeComponent();
            Program.ApplyClassicTheme(this);
            RequireLogin();
            LoadEmails();
            AuditLogger.LogAction("EMAIL_MODULE_OPENED", "Email Terminal module initialized");
        }

        private void InitializeComponent()
        {
            this.Text = "Meisei Email Terminal - Not Logged In";
            this.Size = new Size(1000, 700);
            this.BackColor = Color.FromArgb(212, 208, 200);

            // Toolbar
            var toolbar = new ToolStrip
            {
                BackColor = Color.FromArgb(212, 208, 200),
                Location = new Point(0, 0)
            };

            var newEmailButton = new ToolStripButton("New Email");
            newEmailButton.Click += NewEmail_Click;

            var replyButton = new ToolStripButton("Reply");
            replyButton.Click += Reply_Click;

            var forwardButton = new ToolStripButton("Forward");
            forwardButton.Click += Forward_Click;

            var deleteButton = new ToolStripButton("Delete");
            deleteButton.Click += Delete_Click;

            var refreshButton = new ToolStripButton("Refresh");
            refreshButton.Click += Refresh_Click;

            toolbar.Items.AddRange(new ToolStripItem[] { newEmailButton, replyButton, forwardButton, deleteButton, new ToolStripSeparator(), refreshButton });

            // Folder list (left panel)
            var folderLabel = new Label
            {
                Text = "Folders:",
                Location = new Point(10, 35),
                Size = new Size(180, 20),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold)
            };

            _folderList = new ListBox
            {
                Location = new Point(10, 60),
                Size = new Size(180, 580),
                BackColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };
            _folderList.Items.AddRange(new object[] 
            { 
                "Inbox (3)", 
                "Sent Items", 
                "Drafts", 
                "Deleted Items",
                "Archive",
                "Spam/Junk",
                "Corporate Announcements",
                "HR Notifications",
                "Mandatory Training",
                "Policy Updates"
            });
            _folderList.SelectedIndexChanged += FolderList_SelectedIndexChanged;

            // Email list (center panel)
            var emailListLabel = new Label
            {
                Text = "Messages:",
                Location = new Point(200, 35),
                Size = new Size(380, 20),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold)
            };

            _emailList = new ListView
            {
                Location = new Point(200, 60),
                Size = new Size(380, 580),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };
            _emailList.Columns.Add("From", 120);
            _emailList.Columns.Add("Subject", 180);
            _emailList.Columns.Add("Date", 75);
            _emailList.SelectedIndexChanged += EmailList_SelectedIndexChanged;

            // Email content (right panel)
            var contentLabel = new Label
            {
                Text = "Message Content:",
                Location = new Point(590, 35),
                Size = new Size(390, 20),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold)
            };

            _emailContent = new RichTextBox
            {
                Location = new Point(590, 60),
                Size = new Size(390, 580),
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                Font = new Font("Courier New", 9F)
            };

            this.Controls.AddRange(new Control[] 
            { 
                toolbar, folderLabel, _folderList, emailListLabel, _emailList, contentLabel, _emailContent 
            });
        }

        private void RequireLogin()
        {
            AuditLogger.LogAction("EMAIL_LOGIN_REQUIRED", "User must authenticate to access email");

            using (var loginForm = new Form())
            {
                loginForm.Text = "Meisei Email Terminal - Login Required";
                loginForm.Size = new Size(400, 250);
                loginForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                loginForm.StartPosition = FormStartPosition.CenterScreen;
                loginForm.MaximizeBox = false;
                loginForm.MinimizeBox = false;
                loginForm.BackColor = Color.FromArgb(212, 208, 200);

                var warningLabel = new Label
                {
                    Text = "⚠ CORPORATE EMAIL SYSTEM ⚠\n\nAll email communications are monitored.\nUnauthorized access is prohibited.\n\nPlease enter your credentials:",
                    Location = new Point(20, 20),
                    Size = new Size(350, 80),
                    Font = new Font("Microsoft Sans Serif", 9F),
                    ForeColor = Color.DarkRed
                };

                var usernameLabel = new Label
                {
                    Text = "Username:",
                    Location = new Point(20, 110),
                    Size = new Size(80, 20)
                };

                var usernameBox = new TextBox
                {
                    Location = new Point(110, 108),
                    Size = new Size(250, 20)
                };

                var passwordLabel = new Label
                {
                    Text = "Password:",
                    Location = new Point(20, 140),
                    Size = new Size(80, 20)
                };

                var passwordBox = new TextBox
                {
                    Location = new Point(110, 138),
                    Size = new Size(250, 20),
                    PasswordChar = '*'
                };

                var loginButton = new Button
                {
                    Text = "Login",
                    Location = new Point(200, 175),
                    Size = new Size(80, 25),
                    DialogResult = DialogResult.OK
                };

                var cancelButton = new Button
                {
                    Text = "Cancel",
                    Location = new Point(290, 175),
                    Size = new Size(70, 25),
                    DialogResult = DialogResult.Cancel
                };

                loginForm.Controls.AddRange(new Control[] 
                { 
                    warningLabel, usernameLabel, usernameBox, passwordLabel, passwordBox, loginButton, cancelButton 
                });
                loginForm.AcceptButton = loginButton;
                loginForm.CancelButton = cancelButton;

                var result = loginForm.ShowDialog();
                
                string username = usernameBox.Text;
                if (string.IsNullOrWhiteSpace(username))
                {
                    username = Environment.UserName;
                }

                if (result == DialogResult.OK || result == DialogResult.Cancel)
                {
                    if (result == DialogResult.Cancel)
                    {
                        MessageBox.Show("Login is required to use Email Terminal.\n\nLogging in with default credentials...",
                            "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    ConfirmationHelper.ShowProcessingDialog("Authenticating credentials...", 1200);
                    ConfirmationHelper.ShowProcessingDialog("Verifying access rights...", 1000);
                    ConfirmationHelper.ShowProcessingDialog("Loading mailbox...", 1500);

                    _isLoggedIn = true;
                    this.Text = $"Meisei Email Terminal - {username}@meisei.corp";
                    AuditLogger.LogAction("EMAIL_LOGIN_SUCCESS", $"User: {username}");

                    MessageBox.Show($"Login successful.\n\nWelcome {username}\n\nYou have 10 unread messages.\n\n" +
                                   "READ RECEIPT POLICY:\nAll emails sent from this terminal require read receipts.\n\n" +
                                   "SESSION TIMEOUT: 15 minutes of inactivity\n\n" +
                                   "REMINDER: Check your email at least once per hour per corporate policy.",
                        "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void LoadEmails()
        {
            _emails = new List<Email>
            {
                new Email 
                { 
                    From = "hr@meisei.corp", 
                    Subject = "MANDATORY: New Policy Training", 
                    Date = DateTime.Now.AddHours(-2),
                    Body = "This is a mandatory corporate announcement.\n\nAll employees must complete the new policy training module by end of week.\n\nFailure to comply will result in account suspension.\n\nPlease acknowledge receipt of this email immediately."
                },
                new Email 
                { 
                    From = "it@meisei.corp", 
                    Subject = "System Maintenance - READ IMMEDIATELY", 
                    Date = DateTime.Now.AddHours(-5),
                    Body = "CORPORATE IT DEPARTMENT\n\nScheduled maintenance window:\nDate: Tonight\nTime: 11:00 PM - 3:00 AM\n\nAll systems will be offline.\nSave your work frequently.\n\nDo not attempt to log in during maintenance."
                },
                new Email 
                { 
                    From = "manager@meisei.corp", 
                    Subject = "Your Performance Review is Due", 
                    Date = DateTime.Now.AddDays(-1),
                    Body = "Your quarterly performance review is overdue.\n\nPlease schedule a meeting with HR within 24 hours.\n\nThis is your second reminder.\n\nFailure to respond may affect your performance rating."
                },
                new Email 
                { 
                    From = "noreply@meisei.corp", 
                    Subject = "RE: Timesheet Reminder - Week Ending 09/29", 
                    Date = DateTime.Now.AddDays(-2),
                    Body = "AUTOMATED REMINDER\n\nThis is an automated reminder that your timesheet for the week ending 09/29/2025 is overdue.\n\nPlease submit your timesheet immediately to avoid payroll delays.\n\nTimesheets not submitted by EOD will be escalated to your manager.\n\nDo not reply to this message."
                },
                new Email 
                { 
                    From = "facilities@meisei.corp", 
                    Subject = "Desk Inspection Notice", 
                    Date = DateTime.Now.AddDays(-3),
                    Body = "FACILITIES MANAGEMENT\n\nNotice: Routine desk inspection scheduled for this Friday.\n\nPer corporate policy 5.2.1, all desks must be:\n- Clear of food items\n- Organized and clutter-free\n- Free of personal items exceeding 3 items\n\nNon-compliant desks will be documented.\n\nThank you for your cooperation."
                },
                new Email 
                { 
                    From = "benefits@meisei.corp", 
                    Subject = "Important: Benefits Enrollment Period Closing Soon", 
                    Date = DateTime.Now.AddDays(-4),
                    Body = "BENEFITS DEPARTMENT\n\nThis is a reminder that the annual benefits enrollment period closes in 3 days.\n\nIf you do not make your selections, you will be defaulted to:\n- Basic health coverage\n- No dental\n- No vision\n- Minimum 401(k) contribution\n\nPlease complete your enrollment at: benefits.meisei.corp/portal\n\nQuestions? Submit ticket to helpdesk."
                },
                new Email 
                { 
                    From = "security@meisei.corp", 
                    Subject = "SECURITY ALERT: Badge Policy Violation", 
                    Date = DateTime.Now.AddDays(-5),
                    Body = "CORPORATE SECURITY\n\nOur records indicate your badge was scanned at an unauthorized location on 09/25/2025 at 3:47 PM.\n\nLocation: Building C - Floor 5 (Restricted)\n\nPlease respond to this email within 24 hours with explanation.\n\nFailure to respond will result in:\n- Badge access review\n- Mandatory security training\n- Incident report filed with HR\n\nSecurity Incident #2847"
                },
                new Email 
                { 
                    From = "parking@meisei.corp", 
                    Subject = "Parking Lot Assignment Change", 
                    Date = DateTime.Now.AddDays(-6),
                    Body = "PARKING MANAGEMENT\n\nEffective immediately, your parking spot has been reassigned.\n\nOld Assignment: Lot A, Space 47 (Close to building)\nNew Assignment: Lot D, Space 189 (Far from building)\n\nReason: Seniority-based reallocation\n\nParking map: parking.meisei.corp/map\n\nNo exceptions will be granted."
                },
                new Email 
                { 
                    From = "cafeteria@meisei.corp", 
                    Subject = "Menu Change: Budget Restrictions", 
                    Date = DateTime.Now.AddDays(-7),
                    Body = "CAFETERIA SERVICES\n\nDue to budget constraints, the following items are no longer available:\n- Fresh fruit\n- Premium coffee\n- Salad bar\n\nNew menu consists of:\n- Sandwiches (limited selection)\n- Soup (rotation: Monday-Tomato, Tuesday-Vegetable, Wednesday-Chicken, repeat)\n- Coffee (standard blend only)\n\nPrices remain unchanged."
                },
                new Email 
                { 
                    From = "it@meisei.corp", 
                    Subject = "URGENT: Password Expiration in 1 Day", 
                    Date = DateTime.Now.AddHours(-8),
                    Body = "IT SECURITY\n\nYour network password expires in 1 day.\n\nPassword requirements:\n- Minimum 16 characters\n- Must include: uppercase, lowercase, numbers, special characters\n- Cannot reuse last 24 passwords\n- Cannot contain any dictionary words\n- Cannot contain your name, username, or birthdate\n\nChange password at: passwd.meisei.corp\n\nFailure to change password will result in account lockout."
                }
            };

            RefreshEmailList();
        }

        private void RefreshEmailList()
        {
            _emailList.Items.Clear();
            foreach (var email in _emails)
            {
                var item = new ListViewItem(email.From);
                item.SubItems.Add(email.Subject);
                item.SubItems.Add(email.Date.ToString("MM/dd/yy"));
                item.Tag = email;
                _emailList.Items.Add(item);
            }
        }

        private void FolderList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_folderList.SelectedItem != null)
            {
                AuditLogger.LogAction("EMAIL_FOLDER_CHANGED", $"Folder: {_folderList.SelectedItem}");
                ConfirmationHelper.ShowProcessingDialog($"Loading folder: {_folderList.SelectedItem}...", 1000);
            }
        }

        private void EmailList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_emailList.SelectedItems.Count > 0)
            {
                var email = _emailList.SelectedItems[0].Tag as Email;
                if (email != null)
                {
                    AuditLogger.LogAction("EMAIL_OPENED", $"From: {email.From}, Subject: {email.Subject}");
                    
                    ConfirmationHelper.ShowProcessingDialog("Loading email content...", 800);

                    _emailContent.Text = $"From: {email.From}\n" +
                                        $"To: {Environment.UserName}@meisei.corp\n" +
                                        $"Subject: {email.Subject}\n" +
                                        $"Date: {email.Date:yyyy-MM-dd HH:mm:ss}\n" +
                                        $"Priority: HIGH\n" +
                                        $"Read Receipt: REQUIRED\n" +
                                        $"\n" +
                                        $"─────────────────────────────────\n\n" +
                                        $"{email.Body}\n\n" +
                                        $"─────────────────────────────────\n" +
                                        $"This email has been scanned for viruses.\n" +
                                        $"Corporate monitoring active.\n" +
                                        $"Read receipt will be sent automatically.";

                    // Annoying read receipt
                    MessageBox.Show("Read receipt will be sent to sender.\n\nThis action has been logged.",
                        "Read Receipt", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void NewEmail_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("New Email Button");
            
            if (!ConfirmationHelper.ConfirmAction("compose a new email", 2))
                return;

            ShowEmailComposer();
        }

        private void ShowEmailComposer()
        {
            using (var composeForm = new Form())
            {
                composeForm.Text = "Compose New Email - Step 1 of 3";
                composeForm.Size = new Size(600, 500);
                composeForm.BackColor = Color.FromArgb(212, 208, 200);
                composeForm.StartPosition = FormStartPosition.CenterParent;

                var toLabel = new Label { Text = "To:", Location = new Point(20, 20), Size = new Size(50, 20) };
                var toBox = new TextBox { Location = new Point(80, 18), Size = new Size(480, 20) };

                var subjectLabel = new Label { Text = "Subject:", Location = new Point(20, 50), Size = new Size(50, 20) };
                var subjectBox = new TextBox { Location = new Point(80, 48), Size = new Size(480, 20) };

                var bodyLabel = new Label { Text = "Body:", Location = new Point(20, 80), Size = new Size(50, 20) };
                var bodyBox = new RichTextBox { Location = new Point(80, 80), Size = new Size(480, 300), BorderStyle = BorderStyle.Fixed3D };

                var sendButton = new Button { Text = "Send", Location = new Point(380, 400), Size = new Size(80, 30) };
                sendButton.Click += (s, ev) =>
                {
                    if (string.IsNullOrWhiteSpace(toBox.Text) || string.IsNullOrWhiteSpace(subjectBox.Text))
                    {
                        MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (ConfirmationHelper.ConfirmAction("send this email", 3))
                    {
                        SendEmail(toBox.Text, subjectBox.Text, bodyBox.Text);
                        composeForm.Close();
                    }
                };

                var cancelButton = new Button { Text = "Cancel", Location = new Point(470, 400), Size = new Size(90, 30) };
                cancelButton.Click += (s, ev) => composeForm.Close();

                composeForm.Controls.AddRange(new Control[] 
                { 
                    toLabel, toBox, subjectLabel, subjectBox, bodyLabel, bodyBox, sendButton, cancelButton 
                });

                composeForm.ShowDialog();
            }
        }

        private void SendEmail(string to, string subject, string body)
        {
            ConfirmationHelper.ShowProcessingDialog("Validating recipient address...", 1000);
            ConfirmationHelper.ShowProcessingDialog("Encrypting message (Layer 1/3)...", 900);
            ConfirmationHelper.ShowProcessingDialog("Encrypting message (Layer 2/3)...", 900);
            ConfirmationHelper.ShowProcessingDialog("Encrypting message (Layer 3/3)...", 900);
            ConfirmationHelper.ShowProcessingDialog("Routing through corporate server 1...", 1100);
            ConfirmationHelper.ShowProcessingDialog("Routing through corporate server 2...", 1100);
            ConfirmationHelper.ShowProcessingDialog("Scanning for policy violations...", 1200);
            ConfirmationHelper.ShowProcessingDialog("Requesting read receipt...", 800);
            ConfirmationHelper.ShowProcessingDialog("Delivering message...", 1000);

            MessageBox.Show($"Email sent successfully!\n\n" +
                           $"To: {to}\n" +
                           $"Subject: {subject}\n" +
                           $"Size: {body.Length} bytes\n" +
                           $"Encryption: Triple-layer corporate encryption\n" +
                           $"Sent via: Corporate Email Server 2\n" +
                           $"Read receipt: Requested\n\n" +
                           $"A copy has been saved to your Sent Items folder.\n" +
                           $"This email has been archived for compliance purposes.",
                "Email Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);

            AuditLogger.LogAction("EMAIL_SENT", $"To: {to}, Subject: {subject}");
        }

        private void Reply_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Reply Button");
            MessageBox.Show("Reply functionality requires manager approval.\n\nAll outgoing emails must be approved before sending.\n\nPlease use the approval workflow system.",
                "Approval Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void Forward_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Forward Button");
            MessageBox.Show("Email forwarding is restricted by corporate policy.\n\nOnly authorized personnel may forward internal emails.\n\nPlease submit form EMAIL-FWD-100 for approval.",
                "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void Delete_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Delete Button");
            
            if (_emailList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an email to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ConfirmationHelper.ConfirmAction("delete this email", 4))
                return;

            MessageBox.Show("Corporate emails cannot be permanently deleted.\n\n" +
                           "Emails are moved to Deleted Items folder for 90 days.\n" +
                           "After 90 days, emails are archived for 7 years.\n" +
                           "All emails are subject to legal hold policies.\n\n" +
                           "Deletion request has been logged.",
                "Deletion Policy", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Refresh_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Refresh Button");
            ConfirmationHelper.ShowProcessingDialog("Connecting to mail server...", 1500);
            ConfirmationHelper.ShowProcessingDialog("Checking for new messages...", 1200);
            MessageBox.Show("No new messages.\n\nLast check: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                "Inbox Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private class Email
        {
            public string From { get; set; } = "";
            public string Subject { get; set; } = "";
            public DateTime Date { get; set; }
            public string Body { get; set; } = "";
        }
    }
}
