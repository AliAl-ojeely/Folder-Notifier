using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace FolderNotifier.Views
{
    public partial class DeveloperInfo : Window
    {
        public DeveloperInfo()
        {
            InitializeComponent();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });

            e.Handled = true;
        }
    }
}