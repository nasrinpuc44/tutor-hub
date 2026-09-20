// 📁 Models/TutorProfile.cs
// লোকেশন: Tutorbub/Models/TutorProfile.cs

using System.Collections.Generic;

namespace Tutorbub.Models
{
    public class TutorProfile
    {
        public int Id { get; set; }
        public string TutorId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public List<string> Subjects { get; set; } = new();
        public List<string> Classes { get; set; } = new();
        public string Qualification { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public string Location { get; set; } = string.Empty;
        public string TeachingMode { get; set; } = string.Empty;
        public decimal MonthlyFee { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public string AvailableTime { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public bool IsTopRated { get; set; }
    }
}