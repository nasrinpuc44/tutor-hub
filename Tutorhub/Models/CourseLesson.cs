// 📁 Models/CourseLesson.cs
// লোকেশন: Tutorbub/Models/CourseLesson.cs

using System;

namespace Tutorbub.Models
{
    public class CourseLesson
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int ModuleNumber { get; set; } = 1;
        public int LessonNumber { get; set; } = 1;
        public string Title { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Course? Course { get; set; }
    }
}