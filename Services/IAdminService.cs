using TalepYonetimi.ViewModels;

namespace TalepYonetimi.Services
{
    public interface IAdminService
    {
        // Roles
        Task<List<RoleListViewModel>> GetAllRolesAsync();
        Task<RoleEditViewModel?> GetRoleForEditAsync(Guid id);
        Task<bool> UpdateRolePermissionsAsync(Guid roleId, List<Guid> permissionIds);

        // Users
        Task<List<UserListViewModel>> GetAllUsersAsync();
        Task<UserRoleEditViewModel?> GetUserForRoleEditAsync(Guid userId);
        Task<bool> UpdateUserRoleAsync(Guid userId, Guid roleId);
        Task<bool> ToggleUserActiveAsync(Guid userId);

        // Permissions
        Task<List<PermissionCheckboxViewModel>> GetAllPermissionsAsync();
    }
}
