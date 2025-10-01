using System;
using System.Windows.Forms;
using System.Threading;

namespace MeiseiEnterprise
{
    /// <summary>
    /// Ensures users have to confirm EVERYTHING multiple times
    /// </summary>
    public static class ConfirmationHelper
    {
        public static bool ConfirmAction(string action, int confirmationLevels = 3)
        {
            AuditLogger.LogAction("CONFIRMATION_REQUIRED", $"Action: {action}, Levels: {confirmationLevels}");
            
            string[] confirmationMessages = new[]
            {
                $"Do you want to {action}?",
                $"Are you sure you want to {action}?",
                $"Are you REALLY sure you want to {action}?",
                $"This action cannot be undone easily. Proceed with {action}?",
                $"Final confirmation: {action}?",
                $"Last chance to cancel {action}. Continue?",
                $"Management approval required. Confirm {action}?",
                $"HR has been notified of your intent to {action}. Proceed?"
            };

            for (int i = 0; i < confirmationLevels; i++)
            {
                string message = confirmationMessages[Math.Min(i, confirmationMessages.Length - 1)];
                
                var result = MessageBox.Show(
                    message + $"\n\n(Confirmation {i + 1} of {confirmationLevels})",
                    "Meisei Confirmation System",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                AuditLogger.LogAction("CONFIRMATION_RESPONSE", $"Level {i + 1}: {result}");

                if (result == DialogResult.No)
                {
                    MessageBox.Show(
                        "Action cancelled by user.\n\nYour decision has been logged and HR has been notified.",
                        "Action Cancelled",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return false;
                }

                // Annoying delay between confirmations
                if (i < confirmationLevels - 1)
                {
                    ShowProcessingDialog($"Processing confirmation {i + 1}...", 500);
                }
            }

            ShowProcessingDialog("Verifying authorization level...", 800);
            ShowProcessingDialog("Contacting approval server...", 700);
            ShowProcessingDialog("Finalizing action...", 600);

            return true;
        }

        public static void ShowProcessingDialog(string message, int delayMs)
        {
            using (var form = new Form())
            {
                form.Text = "Meisei Processing";
                form.Size = new System.Drawing.Size(400, 150);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.ControlBox = false;

                var label = new Label
                {
                    Text = message,
                    AutoSize = false,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 10F)
                };

                var progressBar = new ProgressBar
                {
                    Style = ProgressBarStyle.Marquee,
                    MarqueeAnimationSpeed = 30,
                    Dock = DockStyle.Bottom,
                    Height = 23
                };

                form.Controls.Add(label);
                form.Controls.Add(progressBar);

                var timer = new System.Windows.Forms.Timer { Interval = delayMs };
                timer.Tick += (s, e) => { timer.Stop(); form.Close(); };
                timer.Start();

                form.ShowDialog();
            }

            AuditLogger.LogAction("PROCESSING_DIALOG", message);
        }

        public static bool RequireAdminApproval(string action)
        {
            AuditLogger.LogAction("ADMIN_APPROVAL_REQUIRED", action);
            
            using (var form = new Form())
            {
                form.Text = "Administrator Approval Required";
                form.Size = new System.Drawing.Size(450, 220);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label
                {
                    Text = $"The following action requires administrator approval:\n\n{action}\n\nPlease enter administrator credentials:",
                    Location = new System.Drawing.Point(20, 20),
                    Size = new System.Drawing.Size(400, 60),
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
                };

                var usernameLabel = new Label
                {
                    Text = "Username:",
                    Location = new System.Drawing.Point(20, 90),
                    Size = new System.Drawing.Size(80, 20)
                };

                var usernameBox = new TextBox
                {
                    Location = new System.Drawing.Point(110, 88),
                    Size = new System.Drawing.Size(300, 20)
                };

                var passwordLabel = new Label
                {
                    Text = "Password:",
                    Location = new System.Drawing.Point(20, 120),
                    Size = new System.Drawing.Size(80, 20)
                };

                var passwordBox = new TextBox
                {
                    Location = new System.Drawing.Point(110, 118),
                    Size = new System.Drawing.Size(300, 20),
                    PasswordChar = '*'
                };

                var okButton = new Button
                {
                    Text = "Approve",
                    Location = new System.Drawing.Point(250, 150),
                    Size = new System.Drawing.Size(80, 25),
                    DialogResult = DialogResult.OK
                };

                var cancelButton = new Button
                {
                    Text = "Deny",
                    Location = new System.Drawing.Point(340, 150),
                    Size = new System.Drawing.Size(70, 25),
                    DialogResult = DialogResult.Cancel
                };

                form.Controls.AddRange(new Control[] { label, usernameLabel, usernameBox, passwordLabel, passwordBox, okButton, cancelButton });
                form.AcceptButton = okButton;
                form.CancelButton = cancelButton;

                // Accept any credentials (it's fake anyway)
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ShowProcessingDialog("Verifying credentials with corporate server...", 1000);
                    ShowProcessingDialog("Checking authorization level...", 800);
                    MessageBox.Show("Administrator approval granted.\n\nThis approval has been logged.", 
                        "Approval Granted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AuditLogger.LogAction("ADMIN_APPROVAL_GRANTED", $"User: {usernameBox.Text}");
                    return true;
                }

                return false;
            }
        }
    }
}
