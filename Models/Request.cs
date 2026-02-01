using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalepYonetimi.Enums;

namespace TalepYonetimi.Models
{
    public class Request
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Display(Name = "Talep No")]
        [StringLength(20)]
        public string RequestNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık 200 karakteri aşamaz.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(2000, ErrorMessage = "Açıklama 2000 karakteri aşamaz.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen talep türü seçiniz.")]
        [Display(Name = "Talep Türü")]
        public RequestType RequestType { get; set; }

        [Required(ErrorMessage = "Lütfen öncelik seçiniz.")]
        [Display(Name = "Öncelik")]
        public Priority Priority { get; set; }

        [Required]
        [Display(Name = "Durum")]
        public RequestStatus Status { get; set; } = RequestStatus.Draft;

        [Required]
        [Display(Name = "Oluşturan")]
        public Guid CreatedByUserId { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual User? CreatedByUser { get; set; }

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation property
        public virtual ICollection<RequestStatusHistory> StatusHistory { get; set; } = new List<RequestStatusHistory>();
    }
}
