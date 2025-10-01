using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MeiseiEnterprise
{
    internal static class Program
    {
        // P/Invoke for forcing classic Windows theme
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string? pszSubIdList);
        
        [STAThread]
        static void Main()
        {
            // DISABLE visual styles to get that classic Windows 2000 look
            // Application.EnableVisualStyles(); // COMMENTED OUT - This enables modern themes
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Force classic theme globally
            Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NoneEnabled;
            
            // Initialize the corporate surveillance system
            AuditLogger.Initialize();
            
            // Show splash screen with unnecessary delay
            using (var splash = new SplashForm())
            {
                splash.ShowDialog();
            }
            
            Application.Run(new MainForm());
        }
        
        // Helper method to apply classic theme to any control
        public static void ApplyClassicTheme(Control control)
        {
            if (control.Handle != IntPtr.Zero)
            {
                SetWindowTheme(control.Handle, " ", " "); // Empty strings force classic theme
            }
            
            // Apply to all child controls
            foreach (Control child in control.Controls)
            {
                ApplyClassicTheme(child);
            }
        }
    }
}
