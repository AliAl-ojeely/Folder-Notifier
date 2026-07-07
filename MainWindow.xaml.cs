using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using FolderNotifier.Models;
using FolderNotifier.Services;

namespace FolderNotifier.Views
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _dbService;
        public ObservableCollection<AppNote> Notes { get; set; } = new ObservableCollection<AppNote>();

        public bool IsExitApplication { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
            _dbService = new DatabaseService();

            ThemeManager.ApplyTheme(AppSettings.Current.Theme);

            LoadData();
            UpdateLanguageUI();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!IsExitApplication)
            {
                e.Cancel = true;
                this.Hide();

                if (Application.Current is App app)
                {
                    app.ShowNotification("Folder Notifier", "The application is still running in the background. You can access it from the system tray.");
                }
            }
            else
            {
                base.OnClosing(e);
            }
        }

        private void LoadData()
        {
            var allNotes = _dbService.GetAllNotes();
            Notes = new ObservableCollection<AppNote>(allNotes);
            NotesListView.ItemsSource = Notes;
            UpdateUIState();
        }

        private void UpdateUIState()
        {
            int count = NotesListView.Items.Count;

            NotesCountText.Text = string.Format(Languages.Get("NotesCount"), count);
            StatusBarCountText.Text = string.Format(Languages.Get("NotesLoaded"), count);

            if (count == 0)
            {
                NotesListView.Visibility = Visibility.Collapsed;
                EmptyStatePanel.Visibility = Visibility.Visible;
            }
            else
            {
                NotesListView.Visibility = Visibility.Visible;
                EmptyStatePanel.Visibility = Visibility.Collapsed;
            }
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "Select a folder to attach a note",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = dialog.FolderName;
                var addWindow = new AddNoteWindow(selectedPath);

                if (addWindow.ShowDialog() == true)
                {
                    LoadData();
                    ((App)Application.Current).ClearNoteCache();
                }
            }
        }

        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is AppNote note)
            {
                var editWindow = new AddNoteWindow(note);

                if (editWindow.ShowDialog() == true)
                {
                    LoadData();
                    ((App)Application.Current).ClearNoteCache();

                    var title = Languages.Get("UpdateTitle");
                    var msg = string.Format(Languages.Get("UpdateMsg"), note.Title);
                    CustomMessageBox.Show(msg, title, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is AppNote note)
            {
                var title = Languages.Get("DeleteConfirmTitle");
                var msg = string.Format(Languages.Get("DeleteConfirmMsg"), note.FolderPath);
                var result = CustomMessageBox.Show(msg, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _dbService.DeleteNote(note.Id);
                    LoadData();
                    ((App)Application.Current).ClearNoteCache();
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                NotesListView.ItemsSource = Notes;
            }
            else
            {
                var filtered = Notes.Where(n => n.Title.ToLower().Contains(query) || n.FolderPath.ToLower().Contains(query)).ToList();
                NotesListView.ItemsSource = filtered;
            }

            UpdateUIState();
        }

        private void UpdateLanguageUI()
        {
            this.FlowDirection = Languages.CurrentLang == "AR" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            AppTitleText.Text = Languages.Get("AppTitle");
            RefreshBtnText.Text = Languages.Get("RefreshBtn");
            AddNoteBtnText.Text = Languages.Get("AddNoteBtn");

            EmptyTitleText.Text = Languages.Get("EmptyTitle");
            EmptySubText.Text = Languages.Get("EmptySub");
            ReadyText.Text = Languages.Get("Ready");
            SearchPlaceholderText.Text = Languages.Get("SearchPlaceholder");

            UpdateUIState();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            var devInfo = new DeveloperInfo();
            devInfo.Owner = this;
            devInfo.ShowDialog();
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;

            if (settingsWindow.ShowDialog() == true)
            {
                UpdateLanguageUI();
            }
        }
    }
}