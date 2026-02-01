using System.ComponentModel.DataAnnotations;
using TalepYonetimi.Enums;

namespace TalepYonetimi.ViewModels
{
    public class RequestCreateViewModel
    {
        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(2000, ErrorMessage = "Açıklama en fazla 2000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Talep türü seçiniz.")]
        [Display(Name = "Talep Türü")]
        public RequestType RequestType { get; set; }

        [Required(ErrorMessage = "Öncelik seçiniz.")]
        [Display(Name = "Öncelik")]
        public Priority Priority { get; set; }

        [Display(Name = "Durum")]
        public RequestStatus Status { get; set; } = RequestStatus.Draft;
    }

    public class RequestEditViewModel : RequestCreateViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Talep No")]
        public string RequestNumber { get; set; } = string.Empty;
    }

    public class RequestListViewModel
    {
        public Guid Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public RequestType RequestType { get; set; }
        public Priority Priority { get; set; }
        public RequestStatus Status { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class RequestDetailViewModel
    {
        public Guid Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RequestType RequestType { get; set; }
        public Priority Priority { get; set; }
        public RequestStatus Status { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<StatusHistoryViewModel> StatusHistory { get; set; } = new();
        public bool CanEdit { get; set; }
        public bool CanApprove { get; set; }
    }

    public class StatusHistoryViewModel
    {
        public RequestStatus OldStatus { get; set; }
        public RequestStatus NewStatus { get; set; }
        public string? Comment { get; set; }
        public string ChangedByUserName { get; set; } = string.Empty;
        public DateTime ChangedDate { get; set; }
    }

    public class RequestApproveRejectViewModel
    {
        public Guid Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsApprove { get; set; }

        [Display(Name = "Açıklama")]
        public string? Comment { get; set; }
    }

    public class RequestFilterViewModel
    {
        [Display(Name = "Arama")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Durum")]
        public RequestStatus? Status { get; set; }

        [Display(Name = "Öncelik")]
        public Priority? Priority { get; set; }

        [Display(Name = "Başlangıç Tarihi")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        public DateTime? EndDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
