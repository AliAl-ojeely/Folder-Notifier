using System;
using System.Windows;
using FolderNotifier.Models;
using FolderNotifier.Services;

namespace FolderNotifier.Views
{
    public partial class AddNoteWindow : Window
    {
        private readonly DatabaseService _dbService;

        public AddNoteWindow(string selectedPath)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            PathTextBox.Text = selectedPath;
            UpdateLanguageUI();
        }

        public AddNoteWindow(AppNote noteToEdit)
        {
            InitializeComponent();
            _dbService = new DatabaseService();

            PathTextBox.Text = noteToEdit.FolderPath;
            TitleTextBox.Text = noteToEdit.Title;
            ContentTextBox.Text = noteToEdit.Content;

            UpdateLanguageUI();
        }

        private void UpdateLanguageUI()
        {
            this.FlowDirection = Languages.CurrentLang == "AR" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            this.Title = Languages.Get("AddNewNoteWindowTitle");
            AddNoteHeaderText.Text = Languages.Get("AddFolderNoteTitle");
            PathLabelText.Text = Languages.Get("SelectedPathLabel");
            TitleLabelText.Text = Languages.Get("NoteTitleLabel");
            ContentLabelText.Text = Languages.Get("NoteContentLabel");
            CancelBtn.Content = Languages.Get("CancelBtn");
            SaveBtn.Content = Languages.Get("SaveNoteBtn");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string path = PathTextBox.Text.Trim();
            string title = TitleTextBox.Text.Trim();
            string body = ContentTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(path))
            {
                // استبدال MessageBox بـ CustomMessageBox
                CustomMessageBox.Show(Languages.Get("MissingFolderMsg"), Languages.Get("MissingFolderTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body))
            {
                // استبدال MessageBox بـ CustomMessageBox
                CustomMessageBox.Show(Languages.Get("ValidationEmptyMsg"), Languages.Get("ValidationTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _dbService.CreateOrUpdateNote(path, title, body);

            // استبدال MessageBox بـ CustomMessageBox
            CustomMessageBox.Show(Languages.Get("SuccessMsg"), Languages.Get("SuccessTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}