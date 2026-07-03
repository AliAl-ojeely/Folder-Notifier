using System;
using System.Collections.Generic;
using System.Linq;
using FolderNotifier.Data;
using FolderNotifier.Models;

namespace FolderNotifier.Services
{
    public class DatabaseService
    {
        public DatabaseService()
        {
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
            }
        }

        public void CreateOrUpdateNote(string path, string title, string content)
        {
            using (var context = new AppDbContext())
            {
                var existingNote = context.Notes.FirstOrDefault(n => n.FolderPath == path.ToLowerInvariant());

                if (existingNote != null)
                {
                    existingNote.Title = title;
                    existingNote.Content = content;
                    existingNote.UpdatedAt = DateTime.Now;
                }
                else
                {
                    context.Notes.Add(new AppNote
                    {
                        FolderPath = path.ToLowerInvariant(),
                        Title = title,
                        Content = content,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }
                context.SaveChanges();
            }
        }

        public AppNote? GetNoteByPath(string path)
        {
            using (var context = new AppDbContext())
            {
                return context.Notes.FirstOrDefault(n => n.FolderPath == path.ToLowerInvariant());
            }
        }

        public List<AppNote> GetAllNotes()
        {
            using (var context = new AppDbContext())
            {
                return context.Notes.OrderByDescending(n => n.UpdatedAt).ToList();
            }
        }

        public void DeleteNote(int id)
        {
            using (var context = new AppDbContext())
            {
                var note = context.Notes.Find(id);
                if (note != null)
                {
                    context.Notes.Remove(note);
                    context.SaveChanges();
                }
            }
        }
    }
}