using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalepYonetimi.Attributes;
using TalepYonetimi.Constants;
using TalepYonetimi.Enums;
using TalepYonetimi.Services;
using TalepYonetimi.ViewModels;

namespace TalepYonetimi.Controllers
{
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly IAuthService _authService;

        public RequestsController(IRequestService requestService, IAuthService authService)
        {
            _requestService = requestService;
            _authService = authService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        private async Task<bool> CanViewAllRequests()
        {
            return await _authService.HasPermissionAsync(GetUserId(), Permissions.RequestsViewAll);
        }

        // GET: Requests
        public async Task<IActionResult> Index(RequestFilterViewModel filter)
        {
            var userId = GetUserId();
            var canViewAll = await CanViewAllRequests();

            var result = await _requestService.GetRequestsAsync(userId, canViewAll, filter);

            ViewBag.Filter = filter;
            ViewBag.CanViewAll = canViewAll;

            return View(result);
        }

        // GET: Requests/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetUserId();
            var canViewAll = await CanViewAllRequests();

            var dashboard = await _requestService.GetDashboardDataAsync(userId, canViewAll);
            ViewBag.CanViewAll = canViewAll;

            return View(dashboard);
        }

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var userId = GetUserId();
            var canViewAll = await CanViewAllRequests();

            var request = await _requestService.GetRequestByIdAsync(id, userId, canViewAll);

            if (request == null)
            {
                TempData["ErrorMessage"] = "Talep bulunamadı veya bu talebi görüntüleme yetkiniz yok.";
                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        // GET: Requests/Create
        [RequirePermission(Permissions.RequestsCreate)]
        public IActionResult Create()
        {
            return View(new RequestCreateViewModel());
        }

        // POST: Requests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permissions.RequestsCreate)]
        public async Task<IActionResult> Create(RequestCreateViewModel model, string submitButton)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Kaydet veya Onaya Gönder butonuna göre durum belirle
            model.Status = submitButton == "submit" ? RequestStatus.PendingApproval : RequestStatus.Draft;

            var request = await _requestService.CreateRequestAsync(model, GetUserId());

            TempData["SuccessMessage"] = submitButton == "submit"
                ? "Talep oluşturuldu ve onaya gönderildi."
                : "Talep taslak olarak kaydedildi.";

            return RedirectToAction(nameof(Details), new { id = request.Id });
        }

        // GET: Requests/Edit/5
        [RequirePermission(Permissions.RequestsEdit)]
        public async Task<IActionResult> Edit(Guid id)
        {
            var userId = GetUserId();
            var canViewAll = await CanViewAllRequests();

            var request = await _requestService.GetRequestByIdAsync(id, userId, canViewAll);

            if (request == null)
            {
                TempData["ErrorMessage"] = "Talep bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (!request.CanEdit)
            {
                TempData["ErrorMessage"] = "Bu talep düzenlenemez. Sadece taslak durumundaki kendi taleplerinizi düzenleyebilirsiniz.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var model = new RequestEditViewModel
            {
                Id = request.Id,
                RequestNumber = request.RequestNumber,
                Title = request.Title,
                Description = request.Description,
                RequestType = request.RequestType,
                Priority = request.Priority,
                Status = request.Status
            };

            return View(model);
        }

        // POST: Requests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permissions.RequestsEdit)]
        public async Task<IActionResult> Edit(Guid id, RequestEditViewModel model, string submitButton)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var success = await _requestService.UpdateRequestAsync(id, model, GetUserId());

            if (!success)
            {
                TempData["ErrorMessage"] = "Talep güncellenemedi.";
                return View(model);
            }

            // Onaya gönder butonuna basıldıysa
            if (submitButton == "submit")
            {
                await _requestService.SubmitForApprovalAsync(id, GetUserId());
                TempData["SuccessMessage"] = "Talep güncellendi ve onaya gönderildi.";
            }
            else
            {
                TempData["SuccessMessage"] = "Talep başarıyla güncellendi.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Requests/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permissions.RequestsDelete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _requestService.DeleteRequestAsync(id, GetUserId());

            if (success)
            {
                TempData["SuccessMessage"] = "Talep başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Talep silinemedi. Sadece taslak durumundaki kendi taleplerinizi silebilirsiniz.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Requests/Submit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(Guid id)
        {
            var success = await _requestService.SubmitForApprovalAsync(id, GetUserId());

            if (success)
            {
                TempData["SuccessMessage"] = "Talep onaya gönderildi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Talep onaya gönderilemedi.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Requests/Approve/5
        [RequirePermission(Permissions.RequestsApprove)]
        public async Task<IActionResult> Approve(Guid id)
        {
            var userId = GetUserId();
            var request = await _requestService.GetRequestByIdAsync(id, userId, true);

            if (request == null || request.Status != RequestStatus.PendingApproval)
            {
                TempData["ErrorMessage"] = "Talep bulunamadı veya onay bekleyen durumda değil.";
                return RedirectToAction(nameof(Index));
            }

            var model = new RequestApproveRejectViewModel
            {
                Id = request.Id,
                RequestNumber = request.RequestNumber,
                Title = request.Title,
                IsApprove = true
            };

            return View("ApproveReject", model);
        }

        // GET: Requests/Reject/5
        [RequirePermission(Permissions.RequestsReject)]
        public async Task<IActionResult> Reject(Guid id)
        {
            var userId = GetUserId();
            var request = await _requestService.GetRequestByIdAsync(id, userId, true);

            if (request == null || request.Status != RequestStatus.PendingApproval)
            {
                TempData["ErrorMessage"] = "Talep bulunamadı veya onay bekleyen durumda değil.";
                return RedirectToAction(nameof(Index));
            }

            var model = new RequestApproveRejectViewModel
            {
                Id = request.Id,
                RequestNumber = request.RequestNumber,
                Title = request.Title,
                IsApprove = false
            };

            return View("ApproveReject", model);
        }

        // POST: Requests/ApproveReject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveReject(RequestApproveRejectViewModel model)
        {
            // Red için açıklama zorunlu
            if (!model.IsApprove && string.IsNullOrWhiteSpace(model.Comment))
            {
                ModelState.AddModelError("Comment", "Red işlemi için açıklama zorunludur.");
                return View(model);
            }

            bool success;
            if (model.IsApprove)
            {
                success = await _requestService.ApproveRequestAsync(model.Id, GetUserId(), model.Comment);
                TempData["SuccessMessage"] = success ? "Talep onaylandı." : "Talep onaylanamadı.";
            }
            else
            {
                success = await _requestService.RejectRequestAsync(model.Id, GetUserId(), model.Comment!);
                TempData["SuccessMessage"] = success ? "Talep reddedildi." : "Talep reddedilemedi.";
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
    }
}
