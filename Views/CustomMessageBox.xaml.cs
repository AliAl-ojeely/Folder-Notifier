using System.Windows;
using System.Windows.Media;
using FolderNotifier.Services;

namespace FolderNotifier.Views
{
    public partial class CustomMessageBox : Window
    {
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        private CustomMessageBox(string message, string title, MessageBoxButton button, MessageBoxImage image)
        {
            InitializeComponent();

            TitleText.Text = title;
            MessageText.Text = message;

            // تحديد اتجاه النافذة وترجمة الأزرار بناءً على اللغة الحالية
            bool isArabic = Languages.CurrentLang == "AR";
            this.FlowDirection = isArabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            BtnOk.Content = isArabic ? "حسناً" : "OK";
            BtnYes.Content = isArabic ? "نعم" : "Yes";
            BtnNo.Content = isArabic ? "لا" : "No";

            // إعداد الأيقونة
            switch (image)
            {
                case MessageBoxImage.Information:
                    IconText.Text = "ℹ️";
                    break;
                case MessageBoxImage.Warning:
                    IconText.Text = "⚠️";
                    break;
                case MessageBoxImage.Error:
                    IconText.Text = "❌";
                    break;
                case MessageBoxImage.Question:
                    IconText.Text = "❓";
                    break;
                default:
                    IconText.Text = "💬";
                    break;
            }

            // إعداد الأزرار المعروضة
            switch (button)
            {
                case MessageBoxButton.OK:
                    BtnOk.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    BtnYes.Visibility = Visibility.Visible;
                    BtnNo.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                    BtnOk.Visibility = Visibility.Visible;
                    BtnNo.Visibility = Visibility.Visible;
                    BtnNo.Content = isArabic ? "إلغاء" : "Cancel";
                    break;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            this.Close();
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            this.Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            this.Close();
        }

        // الدالة الساكنة (Static) التي سنستدعيها من أي مكان في التطبيق
        public static MessageBoxResult Show(string message, string title, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None)
        {
            var msgBox = new CustomMessageBox(message, title, button, image);
            msgBox.ShowDialog();
            return msgBox.Result;
        }
    }
}