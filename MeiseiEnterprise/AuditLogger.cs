using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MeiseiEnterprise
{
    /// <summary>
    /// Corporate surveillance system - logs EVERYTHING
    /// </summary>
    public static class AuditLogger
    {
        private static StreamWriter? _logWriter;
        private static string _logPath = "";
        private static int _actionCounter = 0;

        public static void Initialize()
        {
            string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "MeiseiEnterprise", "Logs");
            Directory.CreateDirectory(logDir);
            
            _logPath = Path.Combine(logDir, $"audit_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            _logWriter = new StreamWriter(_logPath, true) { AutoFlush = true };
            
            LogAction("APPLICATION_START", "Meisei Enterprise Suite initialized. All actions will be monitored.");
        }

        public static void LogAction(string action, string details)
        {
            _actionCounter++;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] ACTION #{_actionCounter:D6} | {action} | {details}";
            
            _logWriter?.WriteLine(logEntry);
            
            // Randomly show "logging in progress" message to annoy users
            if (_actionCounter % 50 == 0)
            {
                MessageBox.Show($"Corporate audit log entry #{_actionCounter} recorded.\n\nYour actions are being monitored for quality assurance purposes.",
                    "Meisei Audit System", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static void LogMouseMove(int x, int y)
        {
            if (_actionCounter % 100 == 0) // Only log every 100th mouse move to avoid insane file sizes
            {
                LogAction("MOUSE_MOVE", $"Cursor position: ({x}, {y})");
            }
        }

        public static void LogKeyPress(Keys key)
        {
            LogAction("KEY_PRESS", $"Key: {key}");
        }

        public static void LogClick(string control)
        {
            LogAction("MOUSE_CLICK", $"Control: {control}");
        }

        public static void Close()
        {
            LogAction("APPLICATION_EXIT", "User session terminated. All logs saved to corporate database.");
            _logWriter?.Close();
        }

        public static string GetLogPath() => _logPath;
        public static int GetActionCount() => _actionCounter;
    }
}
