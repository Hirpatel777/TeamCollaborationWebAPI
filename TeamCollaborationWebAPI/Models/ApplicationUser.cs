using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TeamCollaborationWebAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? RoleType { get; set; } // e.g., Manager, Developer, Designer

        [Url]
        public string? ProfilePictureUrl { get; set; }

        public DateTime JoinedOn { get; set; } = DateTime.UtcNow;

        // Navigation property (optional)
        public ICollection<TaskItem>? Tasks { get; set; }
    }
}
