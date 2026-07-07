using System;
using System.IO;
using System.Text.Json;

namespace FolderNotifier.Services
{
    public class AppConfig
    {
        public string Language { get; set; } = "EN";
        public bool IsAutoPopupEnabled { get; set; } = true;
        public string Theme { get; set; } = "Default";
        public double WindowLeft { get; set; } = 0;
        public double WindowTop { get; set; } = 0;
    }

    public static class AppSettings
    {
        private static readonly string FolderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FolderNotifier"
        );
        private static readonly string FilePath = Path.Combine(FolderPath, "settings.json");

        private static AppConfig _currentConfig = new AppConfig();

        public static AppConfig Current => _currentConfig;

        static AppSettings()
        {
            LoadSettings();
        }

        public static void LoadSettings()
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    _currentConfig = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                }
                else
                {
                    _currentConfig = new AppConfig();
                    SaveSettings();
                }
            }
            catch
            {
                _currentConfig = new AppConfig();
            }
        }

        public static void SaveSettings()
        {
            try
            {
                string json = JsonSerializer.Serialize(_currentConfig, new JsonSerializerOptions { WriteIndented = true });
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                File.WriteAllText(FilePath, json);
            }
            catch { /* Ignore save errors */ }
        }

        public static (double Left, double Top) LoadWindowPosition()
        {
            return (_currentConfig.WindowLeft, _currentConfig.WindowTop);
        }

        public static void SaveWindowPosition(double left, double top)
        {
            _currentConfig.WindowLeft = left;
            _currentConfig.WindowTop = top;
            SaveSettings();
        }
    }
}