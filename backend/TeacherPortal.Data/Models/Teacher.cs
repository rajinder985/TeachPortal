using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TeacherPortal.Data.Models
{
    public class Teacher : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        // Navigation property
        public ICollection<Student> Students { get; set; } = [];

        // Helper property
        public string FullName => $"{FirstName} {LastName}";
    }
}
