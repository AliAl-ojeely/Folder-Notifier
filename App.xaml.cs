using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls;
using FolderNotifier.Services;
using FolderNotifier.Views;
using FolderNotifier.Models;

namespace FolderNotifier
{
    public partial class App : Application
    {
        private TaskbarIcon? _notifyIcon;
        private ShellWatcherService? _watcher;
        private DatabaseService? _dbService;
        private NoteWindow? _currentPopup;
        private MainWindow? _mainWindow;

        private string _lastPath = string.Empty;
        private Dictionary<string, AppNote?> _cache = new(StringComparer.OrdinalIgnoreCase);

        private static Mutex? _mutex;
        private const string MutexName = "FolderNotifier_SingleInstance_Mutex";
        private const string PipeName = "FolderNotifier_NamedPipe";

        protected override void OnStartup(StartupEventArgs e)
        {
            AppSettings.LoadSettings();
            Languages.CurrentLang = AppSettings.Current.Language;

            bool createdNew;
            _mutex = new Mutex(true, MutexName, out createdNew);

            bool isContextMenuLaunch = e.Args.Length >= 2 && e.Args[0] == "-show";
            string targetPath = isContextMenuLaunch ? e.Args[1] : string.Empty;

            if (!createdNew)
            {
                if (isContextMenuLaunch) SendPathToRunningInstance(targetPath);
                Environment.Exit(0);
                return;
            }

            base.OnStartup(e);

            StartPipeServer();
            _dbService = new DatabaseService();

            _currentPopup = new NoteWindow();
            _currentPopup.Opacity = 0;
            _currentPopup.Show();
            _currentPopup.HideNote();
            _currentPopup.Opacity = 1;

            _watcher = new ShellWatcherService();
            _watcher.FolderChanged += OnFolderChanged;
            _watcher.Start();

            InitializeSystemTray();

            ThemeManager.ApplyTheme(AppSettings.Current.Theme);

            if (isContextMenuLaunch)
            {
                ShowNoteForPath(targetPath);
            }
            else
            {
                ShowMainWindow();
            }
        }

        private void OnFolderChanged(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                _lastPath = string.Empty;
                Application.Current.Dispatcher.InvokeAsync(() => _currentPopup?.HideNote());
                return;
            }

            if (_lastPath == path)
                return;

            _lastPath = path;

            if (!AppSettings.Current.IsAutoPopupEnabled)
            {
                Application.Current.Dispatcher.InvokeAsync(() => _currentPopup?.HideNote());
                return;
            }

            ShowNoteForPath(path);
        }

        private void ShowNoteForPath(string path)
        {
            if (!_cache.TryGetValue(path, out var note))
            {
                note = _dbService?.GetNoteByPath(path);
                _cache[path] = note;
            }

            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (_currentPopup == null || _currentPopup.IsDragging)
                    return;

                if (note == null)
                {
                    _currentPopup.HideNote();
                    return;
                }

                _currentPopup.UpdateContent(note.Title, note.Content);
                _currentPopup.UpdatePosition();
                _currentPopup.ShowNote();
            });
        }

        private void StartPipeServer()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        using var server = new NamedPipeServerStream(PipeName, PipeDirection.In);
                        await server.WaitForConnectionAsync();

                        using var reader = new StreamReader(server);
                        string? requestedPath = await reader.ReadLineAsync();

                        if (!string.IsNullOrWhiteSpace(requestedPath))
                        {
                            ShowNoteForPath(requestedPath);
                        }
                    }
                    catch { }
                }
            });
        }

        private void SendPathToRunningInstance(string path)
        {
            try
            {
                using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
                client.Connect(1000);

                using var writer = new StreamWriter(client);
                writer.WriteLine(path);
                writer.Flush();
            }
            catch { }
        }

        public void ClearNoteCache()
        {
            _cache.Clear();
        }

        private void InitializeSystemTray()
        {
            _notifyIcon = new TaskbarIcon();
            _notifyIcon.ToolTipText = "Folder Notifier";
            _notifyIcon.IconSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Assets/icon.ico"));

            var menu = new ContextMenu();

            bool isArabic = Languages.CurrentLang == "AR";
            var openItem = new MenuItem { Header = isArabic ? "فتح التطبيق" : "Open Folder Notifier" };
            openItem.Click += (s, args) => ShowMainWindow();

            var exitItem = new MenuItem { Header = isArabic ? "إغلاق التطبيق" : "Exit App" };
            exitItem.Click += (s, args) => ShutdownApplication();

            menu.Items.Add(openItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(exitItem);

            menu.FlowDirection = isArabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            _notifyIcon.ContextMenu = menu;
            _notifyIcon.TrayMouseDoubleClick += (s, args) => ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            if (_mainWindow == null || !_mainWindow.IsLoaded)
            {
                _mainWindow = new MainWindow();
            }
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
        }

        private void ShutdownApplication()
        {
            _watcher?.Stop();
            _notifyIcon?.Dispose();
            _mutex?.ReleaseMutex();

            if (_mainWindow != null)
            {
                _mainWindow.IsExitApplication = true;
            }

            Current.Shutdown();
        }

        public void ShowNotification(string title, string message)
        {
            _notifyIcon?.ShowBalloonTip(title, message, BalloonIcon.Info);
        }
    }
}