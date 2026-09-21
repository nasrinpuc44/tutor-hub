// 📁 Models/EnrolledCourseViewModel.cs
// লোকেশন: Tutorbub/Models/EnrolledCourseViewModel.cs

using System;

namespace Tutorbub.Models
{
    public class EnrolledCourseViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseImage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int Lessons { get; set; }
        public string Level { get; set; } = string.Empty;
        public DateTime EnrolledDate { get; set; }
    }
}