using System;
using System.IO;

namespace FolderNotifier.Services
{
    public static class AppSettings
    {
        private static string GetSettingsPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folderPath = Path.Combine(appDataPath, "FolderNotifier");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            return Path.Combine(folderPath, "note_pos.txt");
        }

        public static void SaveWindowPosition(double left, double top)
        {
            try
            {
                File.WriteAllText(GetSettingsPath(), $"{left}|{top}");
            }
            catch { /* Ignore save errors */ }
        }

        public static (double Left, double Top) LoadWindowPosition()
        {
            try
            {
                string path = GetSettingsPath();
                if (File.Exists(path))
                {
                    string[] parts = File.ReadAllText(path).Split('|');
                    if (parts.Length == 2 &&
                        double.TryParse(parts[0], out double left) &&
                        double.TryParse(parts[1], out double top))
                    {
                        return (left, top);
                    }
                }
            }
            catch { /* Ignore load errors */ }
            return (0, 0);
        }
    }
}