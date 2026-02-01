using Microsoft.EntityFrameworkCore;
using TalepYonetimi.Data;
using TalepYonetimi.Enums;
using TalepYonetimi.Models;
using TalepYonetimi.ViewModels;

namespace TalepYonetimi.Services
{
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _context;

        public RequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<RequestListViewModel>> GetRequestsAsync(Guid userId, bool canViewAll, RequestFilterViewModel filter)
        {
            var query = _context.Requests
                .Include(r => r.CreatedByUser)
                .AsQueryable();

            // Kullanıcı sadece kendi taleplerini görebilir, yönetici hepsini
            if (!canViewAll)
            {
                query = query.Where(r => r.CreatedByUserId == userId);
            }

            // Filtreleme
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(r => r.Title.Contains(filter.SearchTerm) || r.RequestNumber.Contains(filter.SearchTerm));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(r => r.Status == filter.Status.Value);
            }

            if (filter.Priority.HasValue)
            {
                query = query.Where(r => r.Priority == filter.Priority.Value);
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(r => r.CreatedDate >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(r => r.CreatedDate <= filter.EndDate.Value.AddDays(1));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.CreatedDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(r => new RequestListViewModel
                {
                    Id = r.Id,
                    RequestNumber = r.RequestNumber,
                    Title = r.Title,
                    RequestType = r.RequestType,
                    Priority = r.Priority,
                    Status = r.Status,
                    CreatedByUserName = r.CreatedByUser != null ? r.CreatedByUser.FirstName + " " + r.CreatedByUser.LastName : "",
                    CreatedDate = r.CreatedDate
                })
                .ToListAsync();

            return new PagedResult<RequestListViewModel>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<RequestDetailViewModel?> GetRequestByIdAsync(Guid id, Guid userId, bool canViewAll)
        {
            var request = await _context.Requests
                .Include(r => r.CreatedByUser)
                .Include(r => r.StatusHistory)
                    .ThenInclude(h => h.ChangedByUser)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return null;

            // Yetki kontrolü
            if (!canViewAll && request.CreatedByUserId != userId)
                return null;

            return new RequestDetailViewModel
            {
                Id = request.Id,
                RequestNumber = request.RequestNumber,
                Title = request.Title,
                Description = request.Description,
                RequestType = request.RequestType,
                Priority = request.Priority,
                Status = request.Status,
                CreatedByUserName = request.CreatedByUser?.FullName ?? "",
                CreatedDate = request.CreatedDate,
                UpdatedDate = request.UpdatedDate,
                CanEdit = request.Status == RequestStatus.Draft && request.CreatedByUserId == userId,
                CanApprove = request.Status == RequestStatus.PendingApproval && canViewAll,
                StatusHistory = request.StatusHistory
                    .OrderByDescending(h => h.ChangedDate)
                    .Select(h => new StatusHistoryViewModel
                    {
                        OldStatus = h.OldStatus,
                        NewStatus = h.NewStatus,
                        Comment = h.Comment,
                        ChangedByUserName = h.ChangedByUser?.FullName ?? "",
                        ChangedDate = h.ChangedDate
                    })
                    .ToList()
            };
        }

        public async Task<Request> CreateRequestAsync(RequestCreateViewModel model, Guid userId)
        {
            var requestNumber = await GenerateRequestNumberAsync();

            var request = new Request
            {
                Id = Guid.NewGuid(),
                RequestNumber = requestNumber,
                Title = model.Title,
                Description = model.Description,
                RequestType = model.RequestType,
                Priority = model.Priority,
                Status = model.Status,
                CreatedByUserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<bool> UpdateRequestAsync(Guid id, RequestEditViewModel model, Guid userId)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.Id == id && r.CreatedByUserId == userId);

            if (request == null)
                return false;

            // Sadece taslak durumundaki talepler güncellenebilir
            if (request.Status != RequestStatus.Draft)
                return false;

            request.Title = model.Title;
            request.Description = model.Description;
            request.RequestType = model.RequestType;
            request.Priority = model.Priority;
            request.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRequestAsync(Guid id, Guid userId)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.Id == id && r.CreatedByUserId == userId);

            if (request == null)
                return false;

            // Sadece taslak durumundaki talepler silinebilir
            if (request.Status != RequestStatus.Draft)
                return false;

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SubmitForApprovalAsync(Guid id, Guid userId)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.Id == id && r.CreatedByUserId == userId);

            if (request == null || request.Status != RequestStatus.Draft)
                return false;

            var oldStatus = request.Status;
            request.Status = RequestStatus.PendingApproval;
            request.UpdatedDate = DateTime.UtcNow;

            // Durum geçmişi ekle
            _context.RequestStatusHistories.Add(new RequestStatusHistory
            {
                Id = Guid.NewGuid(),
                RequestId = request.Id,
                OldStatus = oldStatus,
                NewStatus = RequestStatus.PendingApproval,
                Comment = "Talep onaya gönderildi.",
                ChangedByUserId = userId,
                ChangedDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveRequestAsync(Guid id, Guid userId, string? comment)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.Id == id);

            if (request == null || request.Status != RequestStatus.PendingApproval)
                return false;

            var oldStatus = request.Status;
            request.Status = RequestStatus.Approved;
            request.UpdatedDate = DateTime.UtcNow;

            _context.RequestStatusHistories.Add(new RequestStatusHistory
            {
                Id = Guid.NewGuid(),
                RequestId = request.Id,
                OldStatus = oldStatus,
                NewStatus = RequestStatus.Approved,
                Comment = comment ?? "Talep onaylandı.",
                ChangedByUserId = userId,
                ChangedDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectRequestAsync(Guid id, Guid userId, string comment)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.Id == id);

            if (request == null || request.Status != RequestStatus.PendingApproval)
                return false;

            var oldStatus = request.Status;
            request.Status = RequestStatus.Rejected;
            request.UpdatedDate = DateTime.UtcNow;

            _context.RequestStatusHistories.Add(new RequestStatusHistory
            {
                Id = Guid.NewGuid(),
                RequestId = request.Id,
                OldStatus = oldStatus,
                NewStatus = RequestStatus.Rejected,
                Comment = comment,
                ChangedByUserId = userId,
                ChangedDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(Guid userId, bool canViewAll)
        {
            var query = _context.Requests.AsQueryable();

            if (!canViewAll)
            {
                query = query.Where(r => r.CreatedByUserId == userId);
            }

            var totalRequests = await query.CountAsync();
            var pendingCount = await query.CountAsync(r => r.Status == RequestStatus.PendingApproval);
            var approvedCount = await query.CountAsync(r => r.Status == RequestStatus.Approved);
            var rejectedCount = await query.CountAsync(r => r.Status == RequestStatus.Rejected);
            var draftCount = await query.CountAsync(r => r.Status == RequestStatus.Draft);

            var recentRequests = await query
                .Include(r => r.CreatedByUser)
                .OrderByDescending(r => r.CreatedDate)
                .Take(5)
                .Select(r => new RequestListViewModel
                {
                    Id = r.Id,
                    RequestNumber = r.RequestNumber,
                    Title = r.Title,
                    RequestType = r.RequestType,
                    Priority = r.Priority,
                    Status = r.Status,
                    CreatedByUserName = r.CreatedByUser != null ? r.CreatedByUser.FirstName + " " + r.CreatedByUser.LastName : "",
                    CreatedDate = r.CreatedDate
                })
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalRequests = totalRequests,
                PendingApprovalCount = pendingCount,
                ApprovedCount = approvedCount,
                RejectedCount = rejectedCount,
                DraftCount = draftCount,
                RecentRequests = recentRequests
            };
        }

        private async Task<string> GenerateRequestNumberAsync()
        {
            var today = DateTime.UtcNow;
            var prefix = $"TLP-{today:yyyyMMdd}";

            var lastRequest = await _context.Requests
                .Where(r => r.RequestNumber.StartsWith(prefix))
                .OrderByDescending(r => r.RequestNumber)
                .FirstOrDefaultAsync();

            int sequence = 1;
            if (lastRequest != null)
            {
                var lastSequence = lastRequest.RequestNumber.Split('-').Last();
                if (int.TryParse(lastSequence, out int lastNum))
                {
                    sequence = lastNum + 1;
                }
            }

            return $"{prefix}-{sequence:D4}";
        }
    }
}
