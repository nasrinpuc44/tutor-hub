// 📁 Models/AssignmentSubmission.cs
// লোকেশন: Tutorbub/Models/AssignmentSubmission.cs

using System;

namespace Tutorbub.Models
{
    // ============================================================
    // ===== ASSIGNMENT SUBMISSION MODEL =====
    // ============================================================
    public class AssignmentSubmission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public string DriveLink { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // ============================================================
        // ===== GRADING INFO =====
        // ============================================================
        public int? Marks { get; set; }
        public string? Feedback { get; set; }
        public DateTime? GradedAt { get; set; }
        public int? GradedBy { get; set; }
        public string Status { get; set; } = "Submitted";
        // Status values: Submitted, Graded, Resubmitted, Recheck_Requested

        // ============================================================
        // ===== RESUBMIT / RECHECK TRACKING =====
        // ============================================================
        public int ResubmitCount { get; set; } = 0;
        public int RecheckCount { get; set; } = 0;
        public int PointsSpent { get; set; } = 0;
        public int? FirstMarks { get; set; }

        // ============================================================
        // ===== LATE SUBMISSION TRACKING =====
        // ============================================================
        public DateTime? DueDate { get; set; }
        public bool IsLateSubmission { get; set; } = false;
        public int LatePointsCharged { get; set; } = 0;

        // ============================================================
        // ===== NAVIGATION / DISPLAY HELPERS =====
        // ============================================================
        public string AssignmentTitle { get; set; } = string.Empty;
        public int MilestoneNumber { get; set; }
        public int TotalMarks { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;

        // ============================================================
        // ===== COMPUTED PROPERTIES =====
        // ============================================================

        /// <summary>
        /// Whether this submission has been graded
        /// </summary>
        public bool IsGraded => Marks.HasValue && Status == "Graded";

        /// <summary>
        /// Percentage score (0-100)
        /// </summary>
        public int Percentage => (TotalMarks > 0 && Marks.HasValue)
            ? (int)Math.Round((double)Marks.Value / TotalMarks * 100)
            : 0;

        /// <summary>
        /// Whether student passed (≥ 60%)
        /// </summary>
        public bool IsPassed => IsGraded && Percentage >= PASS_PERCENTAGE;

        /// <summary>
        /// Whether student failed (&lt; 60%)
        /// </summary>
        public bool IsFailed => IsGraded && Percentage < PASS_PERCENTAGE;

        // ============================================================
        // ===== COST CONSTANTS =====
        // ============================================================

        /// <summary>Points cost to resubmit a failed assignment</summary>
        public const int RESUBMIT_COST = 50;

        /// <summary>Points cost to request a recheck/re-grade</summary>
        public const int RECHECK_COST = 50;

        /// <summary>Points cost for late submission (after due date)</summary>
        public const int LATE_SUBMIT_COST = 100;

        /// <summary>Maximum number of resubmits allowed</summary>
        public const int MAX_RESUBMITS = 3;

        /// <summary>Maximum number of rechecks allowed</summary>
        public const int MAX_RECHECKS = 2;

        /// <summary>Minimum percentage to pass an assignment</summary>
        public const int PASS_PERCENTAGE = 60;

        // ============================================================
        // ===== ELIGIBILITY HELPERS =====
        // ============================================================

        /// <summary>
        /// Can the student resubmit this assignment?
        /// Conditions:
        ///   - Must be graded
        ///   - Must have failed (Percentage &lt; 60)
        ///   - Must not exceed MAX_RESUBMITS
        ///   - Must have enough points
        /// </summary>
        public bool CanResubmit(int userPoints)
        {
            if (!IsGraded) return false;
            if (Percentage >= PASS_PERCENTAGE) return false;
            if (ResubmitCount >= MAX_RESUBMITS) return false;
            return userPoints >= RESUBMIT_COST;
        }

        /// <summary>
        /// Can the student request a recheck?
        /// Conditions:
        ///   - Must be graded
        ///   - Must not exceed MAX_RECHECKS
        ///   - Must have enough points
        /// </summary>
        public bool CanRequestRecheck(int userPoints)
        {
            if (!IsGraded) return false;
            if (RecheckCount >= MAX_RECHECKS) return false;
            return userPoints >= RECHECK_COST;
        }

        /// <summary>
        /// Is this submission late? (Submitted after DueDate)
        /// </summary>
        public bool IsLate()
        {
            if (!DueDate.HasValue) return false;
            return SubmittedAt > DueDate.Value;
        }

        /// <summary>
        /// Days late (0 if not late)
        /// </summary>
        public int DaysLate()
        {
            if (!IsLate()) return 0;
            return (int)Math.Ceiling((SubmittedAt - DueDate!.Value).TotalDays);
        }

        /// <summary>
        /// How many more points needed for resubmit?
        /// </summary>
        public int PointsNeededForResubmit(int userPoints)
        {
            int needed = RESUBMIT_COST - userPoints;
            return needed > 0 ? needed : 0;
        }

        /// <summary>
        /// How many more points needed for recheck?
        /// </summary>
        public int PointsNeededForRecheck(int userPoints)
        {
            int needed = RECHECK_COST - userPoints;
            return needed > 0 ? needed : 0;
        }

        /// <summary>
        /// How many more points needed for late submission?
        /// </summary>
        public int PointsNeededForLate(int userPoints)
        {
            int needed = LATE_SUBMIT_COST - userPoints;
            return needed > 0 ? needed : 0;
        }

        /// <summary>
        /// Display-friendly status label
        /// </summary>
        public string StatusLabel
        {
            get
            {
                return Status switch
                {
                    "Submitted" => "Submitted",
                    "Graded" => "Graded",
                    "Resubmitted" => "Resubmitted",
                    "Recheck_Requested" => "Recheck Requested",
                    _ => Status
                };
            }
        }

        /// <summary>
        /// CSS class for status badge
        /// </summary>
        public string StatusCssClass
        {
            get
            {
                return Status switch
                {
                    "Submitted" => "submitted",
                    "Graded" => "graded",
                    "Resubmitted" => "resubmitted",
                    "Recheck_Requested" => "recheck_requested",
                    _ => "submitted"
                };
            }
        }
    }

    // ============================================================
    // ===== ADMIN SUBMISSION VIEW MODEL =====
    // ============================================================
    public class AdminSubmissionViewModel
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }

        // Assignment info
        public string AssignmentTitle { get; set; } = string.Empty;
        public int MilestoneNumber { get; set; }
        public int TotalMarks { get; set; }
        public string CourseName { get; set; } = string.Empty;

        // Student info
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        // Submission
        public string DriveLink { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime SubmittedAt { get; set; }

        // Grading
        public int? Marks { get; set; }
        public string? Feedback { get; set; }
        public DateTime? GradedAt { get; set; }
        public string Status { get; set; } = "Submitted";

        // Resubmit/Recheck tracking
        public int ResubmitCount { get; set; }
        public int RecheckCount { get; set; }
        public int PointsSpent { get; set; }
        public int? FirstMarks { get; set; }

        // Late submission
        public DateTime? DueDate { get; set; }
        public bool IsLateSubmission { get; set; }
        public int LatePointsCharged { get; set; }

        // ============================================================
        // ===== COMPUTED HELPERS =====
        // ============================================================
        public bool IsGraded => Marks.HasValue && Status == "Graded";
        public int Percentage => (TotalMarks > 0 && Marks.HasValue)
            ? (int)Math.Round((double)Marks.Value / TotalMarks * 100)
            : 0;

        public bool IsResubmit => Status == "Resubmitted" || ResubmitCount > 0;
        public bool IsRecheckRequested => Status == "Recheck_Requested";
    }

    // ============================================================
    // ===== POINTS TRANSACTION MODEL =====
    // ============================================================
    public class PointsTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Points { get; set; }
        public string Type { get; set; } = "info";
        // Type values:
        //   "Earned"           — from quiz
        //   "Spent_Resubmit"   — for resubmit
        //   "Spent_Recheck"    — for recheck
        //   "Spent_Late"       — for late submission
        //   "Refunded"         — refund
        //   "AdminAdjusted"    — manual adjust

        public string? Reference { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ============================================================
        // ===== DISPLAY HELPERS =====
        // ============================================================
        public bool IsEarning => Points > 0;
        public bool IsSpending => Points < 0;
        public string FormattedPoints => Points > 0 ? $"+{Points}" : Points.ToString();

        public string TypeLabel
        {
            get
            {
                return Type switch
                {
                    "Earned" => "Earned",
                    "Spent_Resubmit" => "Resubmit Fee",
                    "Spent_Recheck" => "Recheck Fee",
                    "Spent_Late" => "Late Fee",
                    "Refunded" => "Refunded",
                    "AdminAdjusted" => "Admin Adjusted",
                    _ => Type
                };
            }
        }

        public string TypeIcon
        {
            get
            {
                return Type switch
                {
                    "Earned" => "fa-plus-circle",
                    "Spent_Resubmit" => "fa-redo",
                    "Spent_Recheck" => "fa-search",
                    "Spent_Late" => "fa-clock",
                    "Refunded" => "fa-undo",
                    "AdminAdjusted" => "fa-user-cog",
                    _ => "fa-coins"
                };
            }
        }
    }
}