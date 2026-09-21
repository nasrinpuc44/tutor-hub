// 📁 Models/PaymentModel.cs
// লোকেশন: Tutorbub/Models/PaymentModel.cs

using System;

namespace Tutorbub.Models
{
    // ===== কোর্স অর্ডার মডেল =====
    public class CourseOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseImage { get; set; } = string.Empty;
        public decimal Price { get; set; }

        // ===== পেমেন্ট তথ্য =====
        public string PaymentMethod { get; set; } = string.Empty; // bKash, Nagad, Rocket
        public string TransactionId { get; set; } = string.Empty;
        public string SenderMobileNumber { get; set; } = string.Empty;
        public string? PaymentScreenshot { get; set; }

        // ===== স্ট্যাটাস =====
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string? AdminNote { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? VerifiedDate { get; set; }
        public int? VerifiedBy { get; set; }

        // ===== নেভিগেশন =====
        public User? User { get; set; }
        public bool IsEnrolled { get; set; } = false;
    }

    // ===== পেমেন্ট ভিউ মডেল =====
    public class PaymentViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseImage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;

        // ===== পেমেন্ট ফর্ম =====
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string SenderMobileNumber { get; set; } = string.Empty;
    }

    // ===== অ্যাডমিন পেমেন্ট ভিউ মডেল =====
    public class AdminPaymentViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserMobile { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseImage { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string SenderMobileNumber { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;
        public string? AdminNote { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
    }

    // ===== ইউজার এনরোলমেন্ট =====
    public class UserEnrollment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}