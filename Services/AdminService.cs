using Microsoft.EntityFrameworkCore;
using TalepYonetimi.Data;
using TalepYonetimi.Models;
using TalepYonetimi.ViewModels;

namespace TalepYonetimi.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoleListViewModel>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Select(r => new RoleListViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive,
                    UserCount = _context.Users.Count(u => u.RoleId == r.Id),
                    PermissionCount = _context.RolePermissions.Count(rp => rp.RoleId == r.Id)
                })
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<RoleEditViewModel?> GetRoleForEditAsync(Guid id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return null;

            var allPermissions = await _context.Permissions
                .OrderBy(p => p.Name)
                .ToListAsync();

            var rolePermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();

            return new RoleEditViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                Permissions = allPermissions.Select(p => new PermissionCheckboxViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    IsSelected = rolePermissionIds.Contains(p.Id)
                }).ToList()
            };
        }

        public async Task<bool> UpdateRolePermissionsAsync(Guid roleId, List<Guid> permissionIds)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
                return false;

            // Mevcut izinleri kaldır
            _context.RolePermissions.RemoveRange(role.RolePermissions);

            // Yeni izinleri ekle
            foreach (var permissionId in permissionIds)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    PermissionId = permissionId
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserListViewModel>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Select(u => new UserListViewModel
                {
                    Id = u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    RoleName = u.Role != null ? u.Role.Name : "Rol Yok",
                    RoleId = u.RoleId,
                    IsActive = u.IsActive,
                    CreatedDate = u.CreatedDate
                })
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<UserRoleEditViewModel?> GetUserForRoleEditAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            var roles = await _context.Roles
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .Select(r => new RoleSelectItem
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();

            return new UserRoleEditViewModel
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleId = user.RoleId,
                AvailableRoles = roles
            };
        }

        public async Task<bool> UpdateUserRoleAsync(Guid userId, Guid roleId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;

            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);
            if (!roleExists)
                return false;

            user.RoleId = roleId;
            user.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleUserActiveAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;

            user.IsActive = !user.IsActive;
            user.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PermissionCheckboxViewModel>> GetAllPermissionsAsync()
        {
            return await _context.Permissions
                .OrderBy(p => p.Name)
                .Select(p => new PermissionCheckboxViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    IsSelected = false
                })
                .ToListAsync();
        }
    }
}
