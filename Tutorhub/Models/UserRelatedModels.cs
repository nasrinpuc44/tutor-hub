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

    // ❌ পুরনো CourseOrder ক্লাস মুছে ফেলা হয়েছে
    // ✅ নতুন CourseOrder ক্লাস এখন PaymentModel.cs এ আছে

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