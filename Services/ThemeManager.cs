using System.Windows;
using System.Windows.Media;

namespace FolderNotifier.Services
{
    public static class ThemeManager
    {
        public static void ApplyTheme(string theme)
        {
            var res = Application.Current.Resources;

            if (theme == "Dark")
            {
                res["AppBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C1E16"));
                res["CardBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A2A20"));
                res["PrimaryText"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F2E6D8"));
                res["SecondaryText"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BCA893"));
                res["BorderBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5C4233"));
                res["AccentBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C66D5D"));
                res["HoverBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4F3627"));

                res["EmojiGlowColor"] = (Color)ColorConverter.ConvertFromString("#FFFFFF");
            }
            else
            {
                res["AppBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FDFBF7"));
                res["CardBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                res["PrimaryText"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A1B19"));
                res["SecondaryText"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6E5C5A"));
                res["BorderBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1C7B7"));
                res["AccentBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A84C40"));
                res["HoverBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCFAF5"));

                res["EmojiGlowColor"] = Colors.Transparent;
            }
        }
    }
}