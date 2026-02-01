using TalepYonetimi.Models;

namespace TalepYonetimi.Services
{
    public interface IAuthService
    {
        Task<User?> ValidateUserAsync(string email, string password);
        Task<User?> RegisterUserAsync(string firstName, string lastName, string email, string password);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<string>> GetUserPermissionsAsync(Guid userId);
        Task<bool> HasPermissionAsync(Guid userId, string permissionName);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
