using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalepYonetimi.Enums;

namespace TalepYonetimi.Models
{
    public class Request
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Display(Name = "Request No")]
        [StringLength(20)]
        public string RequestNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a request type.")]
        [Display(Name = "Request Type")]
        public RequestType RequestType { get; set; }

        [Required(ErrorMessage = "Please select priority.")]
        [Display(Name = "Priority")]
        public Priority Priority { get; set; }

        [Required]
        [Display(Name = "Status")]
        public RequestStatus Status { get; set; } = RequestStatus.Draft;

        [Required]
        [Display(Name = "Created By")]
        public Guid CreatedByUserId { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual User? CreatedByUser { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation property
        public virtual ICollection<RequestStatusHistory> StatusHistory { get; set; } = new List<RequestStatusHistory>();
    }
}
