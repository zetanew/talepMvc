using TalepYonetimi.Models;
using TalepYonetimi.Enums;
using TalepYonetimi.ViewModels;

namespace TalepYonetimi.Services
{
    public interface IRequestService
    {
        Task<PagedResult<RequestListViewModel>> GetRequestsAsync(Guid userId, bool canViewAll, RequestFilterViewModel filter);
        Task<RequestDetailViewModel?> GetRequestByIdAsync(Guid id, Guid userId, bool canViewAll);
        Task<Request> CreateRequestAsync(RequestCreateViewModel model, Guid userId);
        Task<bool> UpdateRequestAsync(Guid id, RequestEditViewModel model, Guid userId);
        Task<bool> DeleteRequestAsync(Guid id, Guid userId);
        Task<bool> SubmitForApprovalAsync(Guid id, Guid userId);
        Task<bool> ApproveRequestAsync(Guid id, Guid userId, string? comment);
        Task<bool> RejectRequestAsync(Guid id, Guid userId, string comment);
        Task<DashboardViewModel> GetDashboardDataAsync(Guid userId, bool canViewAll);
    }

    public class DashboardViewModel
    {
        public int TotalRequests { get; set; }
        public int PendingApprovalCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int DraftCount { get; set; }
        public List<RequestListViewModel> RecentRequests { get; set; } = new();
    }
}
