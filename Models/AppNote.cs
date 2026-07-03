using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FolderNotifier.Models
{
    public class AppNote
    {
        public int Id { get; set; }
        public string FolderPath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [NotMapped]
        public string EditedVisibility => UpdatedAt > CreatedAt.AddSeconds(2) ? "Visible" : "Collapsed";
    }
}