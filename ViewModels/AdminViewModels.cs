using System.ComponentModel.DataAnnotations;

namespace TalepYonetimi.ViewModels
{
    public class RoleListViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserCount { get; set; }
        public int PermissionCount { get; set; }
        public bool IsActive { get; set; }
    }

    public class RoleEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Rol adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir.")]
        [Display(Name = "Rol Adı")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        public List<PermissionCheckboxViewModel> Permissions { get; set; } = new();
    }

    public class PermissionCheckboxViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSelected { get; set; }
    }

    public class UserListViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class UserRoleEditViewModel
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rol seçimi zorunludur.")]
        [Display(Name = "Rol")]
        public Guid RoleId { get; set; }

        public List<RoleSelectItem> AvailableRoles { get; set; } = new();
    }

    public class RoleSelectItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
