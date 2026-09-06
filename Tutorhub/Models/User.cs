using System;
using System.Collections.Generic;

namespace Tutorbub.Models
{
    public class User
    {
        // ===== বেসিক ইনফো =====
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; } = true;

        // ===== প্রোফাইল ফিল্ড =====
        public string? MobileNumber { get; set; }
        public string? ProfileImage { get; set; }

        // ===== অ্যাডিশনাল ইনফো =====
        public string? Gender { get; set; }
        public string? AgeRange { get; set; }
        public string? PrimaryDeviceType { get; set; }
        public string? YearsOfExperience { get; set; }
        public string? AreaType { get; set; }

        // ===== অ্যাড্রেস =====
        public string? Country { get; set; }
        public string? StreetAddress { get; set; }
        public string? PermanentAddress { get; set; }

        // ===== এডুকেশন =====
        public string? EducationLevel { get; set; }
        public string? CurrentStudyStatus { get; set; }
        public string? ExamDegreeTitle { get; set; }
        public string? InstitutionName { get; set; }
        public string? PassingYear { get; set; }
        public bool? IsCSEStudent { get; set; }

        // ===== ইম্পোর্ট্যান্ট লিংক =====
        public string? CvLink { get; set; }
        public string? GithubProfile { get; set; }
        public string? PortfolioLink { get; set; }
        public string? LinkedInProfile { get; set; }
        public string? ProfileImageLink { get; set; }

        // ===== রিলেশনশিপ =====
        public List<UserSkill> Skills { get; set; } = new List<UserSkill>();
        public List<CourseOrder> CourseOrders { get; set; } = new List<CourseOrder>();
        public List<Certificate> Certificates { get; set; } = new List<Certificate>();
    }
}