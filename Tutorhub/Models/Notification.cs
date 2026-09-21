// 📁 Models/Notification.cs
// লোকেশন: Tutorbub/Models/Notification.cs

using System;

namespace Tutorbub.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "info"; // info, success, warning, danger
        public string Icon { get; set; } = "fa-bell";
        public string? Link { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}