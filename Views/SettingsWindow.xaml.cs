using System.Windows;
using System.Windows.Controls;
using FolderNotifier.Services;

namespace FolderNotifier.Views
{
    public partial class SettingsWindow : Window
    {
        private bool _isLoaded = false;

        public SettingsWindow()
        {
            InitializeComponent();
            LoadCurrentSettingsIntoUI();
            _isLoaded = true;
            UpdateLanguageUI(AppSettings.Current.Language);
        }

        private void LoadCurrentSettingsIntoUI()
        {
            AutoPopupCheckBox.IsChecked = AppSettings.Current.IsAutoPopupEnabled;

            foreach (ComboBoxItem item in LangComboBox.Items)
            {
                if (item.Tag?.ToString() == AppSettings.Current.Language)
                {
                    LangComboBox.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in ThemeComboBox.Items)
            {
                if (item.Tag?.ToString() == AppSettings.Current.Theme)
                {
                    ThemeComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void LangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            if (LangComboBox.SelectedItem is ComboBoxItem item && item.Tag != null)
            {
                UpdateLanguageUI(item.Tag.ToString()!);
            }
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            if (ThemeComboBox.SelectedItem is ComboBoxItem item && item.Tag != null)
            {
                ThemeManager.ApplyTheme(item.Tag.ToString()!);
            }
        }

        private void UpdateLanguageUI(string lang)
        {
            bool isArabic = lang == "AR";
            this.FlowDirection = isArabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            SettingsTitleText.Text = isArabic ? "إعدادات التطبيق" : "Application Settings";
            LangLabelText.Text = isArabic ? "لغة الواجهة" : "Language";
            PopupLabelText.Text = isArabic ? "ظهور الملاحظة تلقائياً عند فتح المجلد" : "Show Notes Automatically";
            ThemeLabelText.Text = isArabic ? "مظهر التطبيق (الثيم)" : "App Theme";
            CancelBtn.Content = isArabic ? "إلغاء" : "Cancel";
            SaveBtn.Content = isArabic ? "حفظ" : "Save";

            ThemeLightItem.Content = isArabic ? "ثيم فاتح" : "Default (Light)";
            ThemeDarkItem.Content = isArabic ? "ثيم غامق" : "Dark Mode";
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Current.IsAutoPopupEnabled = AutoPopupCheckBox.IsChecked ?? true;

            if (LangComboBox.SelectedItem is ComboBoxItem langItem && langItem.Tag != null)
            {
                AppSettings.Current.Language = langItem.Tag.ToString()!;
                Languages.CurrentLang = AppSettings.Current.Language;
            }

            if (ThemeComboBox.SelectedItem is ComboBoxItem themeItem && themeItem.Tag != null)
            {
                AppSettings.Current.Theme = themeItem.Tag.ToString()!;
            }

            AppSettings.SaveSettings();

            bool isArabic = Languages.CurrentLang == "AR";
            CustomMessageBox.Show(
                isArabic ? "تم حفظ الإعدادات بنجاح!" : "Settings saved successfully!",
                isArabic ? "تم الحفظ" : "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            this.DialogResult = true;
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(AppSettings.Current.Theme);
            this.DialogResult = false;
            this.Close();
        }
    }
}