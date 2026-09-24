// 📁 Models/ClassroomViewModel.cs
// লোকেশন: Tutorbub/Models/ClassroomViewModel.cs

using System.Collections.Generic;

namespace Tutorbub.Models
{
    // ============================================================
    // ===== Classroom ViewModel (Main) =====
    // ============================================================
    public class ClassroomViewModel
    {
        // ===== Course info =====
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseImage { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;

        // ===== Module list =====
        public List<ClassroomModule> Modules { get; set; } = new();

        // ===== Active lesson (default = first lesson) =====
        public int ActiveLessonId { get; set; }
        public string ActiveLessonTitle { get; set; } = string.Empty;
        public string ActiveLessonVideoUrl { get; set; } = string.Empty;
        public string ActiveLessonDuration { get; set; } = string.Empty;

        // ============================================================
        // ===== Milestone Assignments (NEW) =====
        // ============================================================

        /// <summary>
        /// যেসব Milestone complete হয়েছে এবং সেই milestone-এর Assignment Published,
        /// সেগুলো এই list-এ থাকবে। User Classroom-এ এগুলো দেখতে পাবে।
        /// </summary>
        public List<MilestoneAssignment> AvailableAssignments { get; set; } = new();

        /// <summary>
        /// কতটি Module complete হলে ১টি Milestone complete হবে।
        /// Default = 4 (Admin Panel থেকে পরিবর্তন করা যায়)।
        /// </summary>
        public int ModulesPerMilestone { get; set; } = 4;

        /// <summary>
        /// ইউজার এখন পর্যন্ত যেসব Milestone complete করেছে (1, 2, 3, ...)।
        /// </summary>
        public List<int> CompletedMilestones { get; set; } = new();
    }

    // ============================================================
    // ===== Classroom Module =====
    // ============================================================
    public class ClassroomModule
    {
        // ===== Module meta =====
        public int ModuleNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TotalDuration { get; set; } = string.Empty;

        // ===== Lesson counts =====
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }

        // ===== Lesson list =====
        public List<ClassroomLesson> Lessons { get; set; } = new();

        // ============================================================
        // ===== Module Completion & Quiz (NEW) =====
        // ============================================================

        /// <summary>
        /// এই Module-এর সব lesson complete হয়েছে কিনা।
        /// </summary>
        public bool IsModuleCompleted { get; set; }

        /// <summary>
        /// এই Module-এর Quiz ইউজারের জন্য available কিনা।
        /// (শর্ত: module complete + quiz published)
        /// </summary>
        public bool QuizAvailable { get; set; }

        /// <summary>
        /// Quiz-এর ID (যদি QuizAvailable = true হয়)।
        /// </summary>
        public int? QuizId { get; set; }

        /// <summary>
        /// Quiz-এর Title (Display-এর জন্য)।
        /// </summary>
        public string? QuizTitle { get; set; }
    }

    // ============================================================
    // ===== Classroom Lesson =====
    // ============================================================
    public class ClassroomLesson
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
    }
}