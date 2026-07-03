using System;
using System.Data.SQLite;

namespace FolderNotifier
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string dbPath)
        {
            _connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS FolderNotes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FolderPath TEXT UNIQUE NOT NULL,
                        NoteText TEXT NOT NULL
                    );";
                command.ExecuteNonQuery();
            }
        }

        public void SaveNote(string path, string text)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = @"
                    INSERT INTO FolderNotes (FolderPath, NoteText) 
                    VALUES (@path, @text)
                    ON CONFLICT(FolderPath) DO UPDATE SET NoteText = @text;";

                command.Parameters.AddWithValue("@path", path.ToLowerInvariant());
                command.Parameters.AddWithValue("@text", text);

                command.ExecuteNonQuery();
            }
        }

        public string? GetNoteForFolder(string path)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT NoteText FROM FolderNotes WHERE FolderPath = @path;";
                command.Parameters.AddWithValue("@path", path.ToLowerInvariant());

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                }
            }
            return null;
        }
    }
}