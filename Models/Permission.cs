using System.ComponentModel.DataAnnotations;

namespace TalepYonetimi.Models
{
    public class Permission
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Module { get; set; } = string.Empty; // Requests, Users, Dashboard

        // Navigation property
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
