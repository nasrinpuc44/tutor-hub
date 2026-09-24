// 📁 Models/Quiz.cs
using System;
using System.Collections.Generic;

namespace Tutorbub.Models
{
    public class ModuleQuiz
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int ModuleNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PassingScore { get; set; } = 60;
        public int TimeLimitMinutes { get; set; } = 0;
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public List<QuizQuestionItem> Questions { get; set; } = new();
        public int QuestionCount { get; set; }
        public int TotalMarks { get; set; }
    }

    public class QuizQuestionItem
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public string CorrectOption { get; set; } = "A";
        public int Marks { get; set; } = 1;
        public int QuestionOrder { get; set; } = 1;
    }

    public class QuizAttempt
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public int CourseId { get; set; }
        public int Score { get; set; }
        public int TotalMarks { get; set; }
        public decimal Percentage { get; set; }
        public bool Passed { get; set; }
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

        // Display helper
        public string QuizTitle { get; set; } = string.Empty;
        public int ModuleNumber { get; set; }
    }

    // ===== Assignment =====
    public class MilestoneAssignment
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int MilestoneNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Instructions { get; set; }
        public int TotalMarks { get; set; } = 100;
        public int DueDays { get; set; } = 7;
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    // ===== ViewModel for Course Quiz/Assignment Manage =====
    public class CourseQuizAssignmentViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public List<ModuleQuiz> Quizzes { get; set; } = new();
        public List<MilestoneAssignment> Assignments { get; set; } = new();
        public int ModulesPerMilestone { get; set; } = 4;
    }

    // ===== ViewModel for Student Quiz Taking =====
    public class TakeQuizViewModel
    {
        public int QuizId { get; set; }
        public int CourseId { get; set; }
        public int ModuleNumber { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PassingScore { get; set; }
        public int TimeLimitMinutes { get; set; }
        public List<QuizQuestionItem> Questions { get; set; } = new();
        public QuizAttempt? PreviousAttempt { get; set; }
    }
}