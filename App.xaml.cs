using System;
using System.Collections.Generic;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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

            _mainWindow = new MainWindow();
            _mainWindow.Show();
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
            var openItem = new MenuItem { Header = "Open Folder Notifier" };
            openItem.Click += (s, args) => ShowMainWindow();
            var exitItem = new MenuItem { Header = "Exit App" };
            exitItem.Click += (s, args) => ShutdownApplication();
            menu.Items.Add(openItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(exitItem);

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