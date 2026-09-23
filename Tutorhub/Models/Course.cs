// 📁 Models/Course.cs
using System;

namespace Tutorbub.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Students { get; set; } = 0;
        public double Rating { get; set; } = 0;
        public string Instructor { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int Lessons { get; set; } = 0;
        public string Level { get; set; } = string.Empty;
        public bool IsNew { get; set; } = true;
        public bool IsPopular { get; set; } = false;
        public bool IsEnrollmentOpen { get; set; } = true;
        public string? Features { get; set; }   // ✅ নতুন - কমা দিয়ে আলাদা করা স্ট্রিং
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}