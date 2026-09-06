using System.Collections.Generic;

namespace Tutorbub.Models
{
    public class CourseDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Students { get; set; }
        public double Rating { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int Lessons { get; set; }
        public string Level { get; set; } = string.Empty;
        public bool IsNew { get; set; }
        public bool IsPopular { get; set; }
        public List<string> Features { get; set; } = new List<string>();
    }
}