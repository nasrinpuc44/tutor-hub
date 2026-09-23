// 📁 Models/Notice.cs
// লোকেশন: Tutorbub/Models/Notice.cs

using System;

namespace Tutorbub.Models
{
    public class Notice
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = "News";   // News, Workshop, Course, Event, Result
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
        public bool IsAnnouncement { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}