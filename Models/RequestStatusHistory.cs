using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalepYonetimi.Enums;

namespace TalepYonetimi.Models
{
    public class RequestStatusHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid RequestId { get; set; }

        [ForeignKey("RequestId")]
        public virtual Request? Request { get; set; }

        [Required]
        [Display(Name = "Old Status")]
        public RequestStatus OldStatus { get; set; }

        [Required]
        [Display(Name = "New Status")]
        public RequestStatus NewStatus { get; set; }

        [StringLength(500)]
        [Display(Name = "Comment")]
        public string? Comment { get; set; }

        [Required]
        [Display(Name = "Changed By")]
        public Guid ChangedByUserId { get; set; }

        [ForeignKey("ChangedByUserId")]
        public virtual User? ChangedByUser { get; set; }

        [Display(Name = "Changed Date")]
        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;
    }
}
