using System;

namespace Tutorbub.Models
{
    public class UserSkill
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public int ExperienceInYear { get; set; }
        public string? ProjectLinks { get; set; }
        public User? User { get; set; }
    }

    public class CourseOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string? Action { get; set; }
        public User? User { get; set; }
    }

    public class Certificate
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CertificateName { get; set; } = string.Empty;
        public DateTime IssuedDate { get; set; }
        public string? CertificateUrl { get; set; }
        public User? User { get; set; }
    }
}