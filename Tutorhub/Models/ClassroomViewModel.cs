// 📁 Models/ClassroomViewModel.cs
// লোকেশন: Tutorbub/Models/ClassroomViewModel.cs

using System.Collections.Generic;

namespace Tutorbub.Models
{
    public class ClassroomViewModel
    {
        // Course info
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseImage { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;

        // Module list
        public List<ClassroomModule> Modules { get; set; } = new();

        // Active lesson (default = first lesson)
        public int ActiveLessonId { get; set; }
        public string ActiveLessonTitle { get; set; } = string.Empty;
        public string ActiveLessonVideoUrl { get; set; } = string.Empty;
        public string ActiveLessonDuration { get; set; } = string.Empty;
    }

    public class ClassroomModule
    {
        public int ModuleNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TotalDuration { get; set; } = string.Empty;
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public List<ClassroomLesson> Lessons { get; set; } = new();
    }

    public class ClassroomLesson
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
    }
}