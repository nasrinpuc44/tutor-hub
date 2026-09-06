using System;
using System.Collections.Generic;

namespace Tutorbub.Models
{
    public class TeacherRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string SubjectExpertise { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string TeachingStyle { get; set; } = string.Empty;
        public string AvailableDays { get; set; } = string.Empty;
        public string PreferredTime { get; set; } = string.Empty;
        public string HourlyRate { get; set; } = string.Empty;
        public string CvLink { get; set; } = string.Empty;
        public string WhyTeach { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public DateTime? ResponseDate { get; set; }
        public string? AdminNote { get; set; }
        public User? User { get; set; }
    }

    public class TeacherDashboardViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string SubjectExpertise { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string ProfileImageLink { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public int CompletedSessions { get; set; }
        public double Rating { get; set; }
        public List<TeacherCourse>? Courses { get; set; }
        public List<TeacherStudent>? RecentStudents { get; set; }
    }

    public class TeacherCourse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class TeacherStudent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }
}