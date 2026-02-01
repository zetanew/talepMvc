using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalepYonetimi.Attributes;
using TalepYonetimi.Constants;
using TalepYonetimi.Services;
using TalepYonetimi.ViewModels;

namespace TalepYonetimi.Controllers
{
    [Authorize]
    [RequirePermission(Permissions.AdminPanel)]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IAuthService _authService;

        public AdminController(IAdminService adminService, IAuthService authService)
        {
            _adminService = adminService;
            _authService = authService;
        }

        // ROLES

        [HttpGet]
        public async Task<IActionResult> Roles()
        {
            var roles = await _adminService.GetAllRolesAsync();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(Guid id)
        {
            var model = await _adminService.GetRoleForEditAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Rol bulunamadı.";
                return RedirectToAction(nameof(Roles));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(Guid id, List<Guid> selectedPermissions)
        {
            var result = await _adminService.UpdateRolePermissionsAsync(id, selectedPermissions);
            if (result)
            {
                TempData["SuccessMessage"] = "Rol izinleri başarıyla güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Rol izinleri güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Roles));
        }

        // USERS

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _adminService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserRole(Guid id)
        {
            var model = await _adminService.GetUserForRoleEditAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Users));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRole(UserRoleEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Rolleri tekrar yükle
                var refreshedModel = await _adminService.GetUserForRoleEditAsync(model.UserId);
                if (refreshedModel != null)
                {
                    model.AvailableRoles = refreshedModel.AvailableRoles;
                }
                return View(model);
            }

            var result = await _adminService.UpdateUserRoleAsync(model.UserId, model.RoleId);
            if (result)
            {
                TempData["SuccessMessage"] = "Kullanıcı rolü başarıyla güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı rolü güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserActive(Guid id)
        {
            var result = await _adminService.ToggleUserActiveAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Kullanıcı durumu başarıyla güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı durumu güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Users));
        }
    }
}
