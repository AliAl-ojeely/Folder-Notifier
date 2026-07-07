using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Reflection;

namespace FolderNotifier.Services
{
    public static class ContextMenuService
    {
        private const string MenuName = "FolderNotifier";
        private const string MenuText = "Show Folder Note";
        private const string MenuIcon = "shell32.dll,269";

        public static void RegisterContextMenu()
        {
            try
            {
                string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? Assembly.GetExecutingAssembly().Location;

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey($@"Software\Classes\Directory\Background\shell\{MenuName}"))
                {
                    key.SetValue("", MenuText);
                    key.SetValue("Icon", MenuIcon);
                    using (RegistryKey commandKey = key.CreateSubKey("command"))
                    {
                        commandKey.SetValue("", $"\"{exePath}\" -show \"%V\"");
                    }
                }

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey($@"Software\Classes\Directory\shell\{MenuName}"))
                {
                    key.SetValue("", MenuText);
                    key.SetValue("Icon", MenuIcon);
                    using (RegistryKey commandKey = key.CreateSubKey("command"))
                    {
                        commandKey.SetValue("", $"\"{exePath}\" -show \"%1\"");
                    }
                }
            }
            catch { }
        }

        public static void UnregisterContextMenu()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree($@"Software\Classes\Directory\Background\shell\{MenuName}", false);
                Registry.CurrentUser.DeleteSubKeyTree($@"Software\Classes\Directory\shell\{MenuName}", false);
            }
            catch { }
        }
    }
}