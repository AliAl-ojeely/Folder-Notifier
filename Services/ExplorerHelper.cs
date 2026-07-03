using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FolderNotifier.Services
{
    public static class ExplorerHelper
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static string? GetActiveExplorerPath()
        {
            try
            {
                var hwnd = GetForegroundWindow();
                if (hwnd == IntPtr.Zero) return null;

                GetWindowThreadProcessId(hwnd, out uint activeProcessId);
                if (activeProcessId == (uint)System.Diagnostics.Process.GetCurrentProcess().Id)
                {
                    return "APP_FOCUSED";
                }

                StringBuilder className = new StringBuilder(256);
                GetClassName(hwnd, className, className.Capacity);
                if (className.ToString() != "CabinetWClass")
                    return null;

                var shellWindows = new SHDocVw.ShellWindows();
                foreach (SHDocVw.InternetExplorer window in shellWindows)
                {
                    if ((IntPtr)window.HWND == hwnd)
                    {
                        if (window.LocationURL.StartsWith("file:///"))
                        {
                            return new Uri(window.LocationURL).LocalPath;
                        }
                    }
                }
            }
            catch { }

            return null;
        }
    }
}