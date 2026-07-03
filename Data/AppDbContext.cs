using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using FolderNotifier.Models;

namespace FolderNotifier.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<AppNote> Notes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(appData, "FolderNotifier");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string dbPath = Path.Combine(folderPath, "FolderNotes.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppNote>()
                .HasIndex(n => n.FolderPath)
                .IsUnique();
        }
    }
}