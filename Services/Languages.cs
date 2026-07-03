using System.Collections.Generic;

namespace FolderNotifier.Services
{
    public static class Languages
    {
        public static string CurrentLang { get; set; } = "EN";

        private static readonly Dictionary<string, Dictionary<string, string>> Texts = new()
        {
            {
                "EN", new Dictionary<string, string>
                {
                    // MainWindow Texts
                    { "AppTitle", "My Folder Notes" },
                    { "NotesCount", "{0} Notes" },
                    { "RefreshBtn", "🔄 Refresh" },
                    { "AddNoteBtn", "Add Note" },
                    { "EmptyTitle", "No folder notes found." },
                    { "EmptySub", "Click 'Add Note' to create your first note." },
                    { "Ready", "Ready" },
                    { "NotesLoaded", "{0} Notes Loaded" },
                    { "LangToggle", "AR" },
                    { "SearchPlaceholder", "Search a Note ..." },
                    
                    // Messages & Errors
                    { "DeleteConfirmTitle", "Confirm Deletion" },
                    { "DeleteConfirmMsg", "Are you sure you want to delete the note for:\n{0}?" },
                    { "UpdateTitle", "Note Updated" },
                    { "UpdateMsg", "The note for '{0}' has been successfully updated." },
                    
                    // AddNoteWindow UI Texts (NEW)
                    { "AddNewNoteWindowTitle", "Add New Note" },
                    { "AddFolderNoteTitle", "Add Folder Note" },
                    { "SelectedPathLabel", "Selected Path:" },
                    { "NoteTitleLabel", "Note Title:" },
                    { "NoteContentLabel", "Note Content:" },
                    { "CancelBtn", "Cancel" },
                    { "SaveNoteBtn", "Save Note" },

                    // AddNoteWindow Messages (Reused existing keys)
                    { "MissingFolderTitle", "Missing Folder" },
                    { "MissingFolderMsg", "Please select a folder path first." },
                    { "ValidationTitle", "Validation Error" },
                    { "ValidationEmptyMsg", "Title and Note Body cannot be empty. Please fill in both fields." },
                    { "SuccessTitle", "Success" },
                    { "SuccessMsg", "Note saved successfully!" }
                }
            },
            {
                "AR", new Dictionary<string, string>
                {
                    { "AppTitle", "ملاحظات المجلدات" },
                    { "NotesCount", "{0} ملاحظات" },
                    { "RefreshBtn", "🔄 تحديث" },
                    { "AddNoteBtn", "إضافة ملاحظة" },
                    { "EmptyTitle", "لا توجد ملاحظات." },
                    { "EmptySub", "انقر على 'إضافة ملاحظة' لإنشاء ملاحظتك الأولى." },
                    { "Ready", "مستعد" },
                    { "NotesLoaded", "تم تحميل {0} ملاحظات" },
                    { "LangToggle", "EN" },
                    { "SearchPlaceholder", "البحث عن ملاحظة ..." },

                    { "DeleteConfirmTitle", "تأكيد الحذف" },
                    { "DeleteConfirmMsg", "هل أنت متأكد أنك تريد حذف الملاحظة الخاصة بمسار:\n{0}؟" },
                    { "UpdateTitle", "تم التحديث" },
                    { "UpdateMsg", "تم تحديث الملاحظة '{0}' بنجاح." },
                    
                    // AddNoteWindow UI Texts (NEW)
                    { "AddNewNoteWindowTitle", "إضافة ملاحظة جديدة" },
                    { "AddFolderNoteTitle", "إضافة ملاحظة مجلد" },
                    { "SelectedPathLabel", "المسار المحدد:" },
                    { "NoteTitleLabel", "عنوان الملاحظة:" },
                    { "NoteContentLabel", "محتوى الملاحظة:" },
                    { "CancelBtn", "إلغاء" },
                    { "SaveNoteBtn", "حفظ الملاحظة" },

                    // AddNoteWindow Messages (Reused existing keys)
                    { "MissingFolderTitle", "مجلد مفقود" },
                    { "MissingFolderMsg", "يرجى تحديد مسار المجلد أولاً." },
                    { "ValidationTitle", "خطأ في الإدخال" },
                    { "ValidationEmptyMsg", "لا يمكن أن يكون العنوان والمحتوى فارغين. يرجى تعبئة كلا الحقلين." },
                    { "SuccessTitle", "نجاح" },
                    { "SuccessMsg", "تم حفظ الملاحظة بنجاح!" }
                }
            }
        };

        public static string Get(string key)
        {
            if (Texts.ContainsKey(CurrentLang) && Texts[CurrentLang].ContainsKey(key))
            {
                return Texts[CurrentLang][key];
            }
            return key;
        }
    }
}