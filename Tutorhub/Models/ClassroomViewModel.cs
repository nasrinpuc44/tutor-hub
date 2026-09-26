// 📁 Models/ClassroomViewModel.cs
// লোকেশন: Tutorbub/Models/ClassroomViewModel.cs

using System.Collections.Generic;
using System.Linq;

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

        // ===== Active lesson =====
        public int ActiveLessonId { get; set; }
        public string ActiveLessonTitle { get; set; } = string.Empty;
        public string ActiveLessonVideoUrl { get; set; } = string.Empty;
        public string ActiveLessonDuration { get; set; } = string.Empty;

        // ============================================================
        // ===== Milestone Assignments =====
        // ============================================================
        public List<MilestoneAssignment> AvailableAssignments { get; set; } = new();
        public int ModulesPerMilestone { get; set; } = 4;
        public List<int> CompletedMilestones { get; set; } = new();

        // ============================================================
        // ===== ✅ Milestone Grouping Helper =====
        // ============================================================
        /// <summary>
        /// Module গুলোকে Milestone অনুযায়ী Group করে।
        /// প্রতি N টি module = 1 Milestone।
        /// </summary>
        public List<MilestoneGroup> GetMilestoneGroups()
        {
            var groups = new List<MilestoneGroup>();
            if (Modules == null || Modules.Count == 0) return groups;

            int perMilestone = ModulesPerMilestone > 0 ? ModulesPerMilestone : 4;
            int maxModule = Modules.Max(m => m.ModuleNumber);
            int maxMilestone = (int)System.Math.Ceiling((double)maxModule / perMilestone);

            for (int ms = 1; ms <= maxMilestone; ms++)
            {
                int startMod = (ms - 1) * perMilestone + 1;
                int endMod = ms * perMilestone;

                var modulesInGroup = Modules
                    .Where(m => m.ModuleNumber >= startMod && m.ModuleNumber <= endMod)
                    .OrderBy(m => m.ModuleNumber)
                    .ToList();

                if (modulesInGroup.Count == 0) continue;

                bool allComplete = modulesInGroup.All(m => m.IsModuleCompleted);

                var assignment = AvailableAssignments?
                    .FirstOrDefault(a => a.MilestoneNumber == ms);

                groups.Add(new MilestoneGroup
                {
                    MilestoneNumber = ms,
                    StartModule = startMod,
                    EndModule = endMod,
                    Modules = modulesInGroup,
                    IsComplete = allComplete,
                    HasAssignment = assignment != null,
                    Assignment = assignment,
                    IsCurrentMilestone = modulesInGroup.Any(m => !m.IsModuleCompleted)
                });
            }

            return groups;
        }
    }

    // ============================================================
    // ===== Milestone Group =====
    // ============================================================
    public class MilestoneGroup
    {
        public int MilestoneNumber { get; set; }
        public int StartModule { get; set; }
        public int EndModule { get; set; }
        public List<ClassroomModule> Modules { get; set; } = new();

        public bool IsComplete { get; set; }
        public bool HasAssignment { get; set; }
        public MilestoneAssignment? Assignment { get; set; }
        public bool IsCurrentMilestone { get; set; }

        public int CompletedCount => Modules.Count(m => m.IsModuleCompleted);
        public int TotalCount => Modules.Count;
    }

    // ============================================================
    // ===== Classroom Module =====
    // ============================================================
    public class ClassroomModule
    {
        public int ModuleNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TotalDuration { get; set; } = string.Empty;
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public List<ClassroomLesson> Lessons { get; set; } = new();

        public bool IsModuleCompleted { get; set; }
        public bool QuizAvailable { get; set; }
        public int? QuizId { get; set; }
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