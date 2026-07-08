using FolderNotifier.Services;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Linq;

namespace FolderNotifier.Views
{
    public partial class NoteWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern void ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOACTIVATE = 0x0010;

        private bool _isDragging = false;
        public bool IsDragging => _isDragging;

        public NoteWindow()
        {
            InitializeComponent();

            this.Topmost = true;
            this.ShowActivated = false;
            this.Visibility = Visibility.Collapsed;

            this.Loaded += (s, e) =>
            {
                var handle = new WindowInteropHelper(this).Handle;
                SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            };
        }

        public void UpdateContent(string title, string message)
        {
            NoteTitleDisplay.Text = title;
            NoteTextDisplay.Text = message;

            bool isArabic = IsArabicText(title) || IsArabicText(message);

            this.FlowDirection = isArabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        private bool IsArabicText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return text.Any(c => c >= 0x0600 && c <= 0x06FF);
        }

        public void UpdatePosition()
        {
            var (savedLeft, savedTop) = AppSettings.LoadWindowPosition();

            if (savedLeft != 0 && savedTop != 0)
            {
                this.Left = savedLeft;
                this.Top = savedTop;
            }
            else
            {
                this.Left = (SystemParameters.WorkArea.Width - this.Width) / 8;
                this.Top = (SystemParameters.WorkArea.Height - this.Height) / 8;
            }
        }

        public void HideNote()
        {
            this.Visibility = Visibility.Collapsed;
            this.IsHitTestVisible = false;
        }

        public void ShowNote()
        {
            this.Visibility = Visibility.Visible;
            this.IsHitTestVisible = true;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Topmost = false;
                this.Topmost = true;

                var handle = new WindowInteropHelper(this).Handle;
                SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);

                this.Activate();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void MainContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragging = true;
                ReleaseCapture();
                SendMessage(new WindowInteropHelper(this).Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                _isDragging = false;

                AppSettings.SaveWindowPosition(this.Left, this.Top);
            }
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.SaveWindowPosition(this.Left, this.Top);
            HideNote();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            AppSettings.SaveWindowPosition(this.Left, this.Top);
        }
    }
}