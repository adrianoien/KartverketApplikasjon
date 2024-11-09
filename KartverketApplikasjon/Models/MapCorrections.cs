using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KartverketApplikasjon.Models
{
    public class MapCorrections
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Latitude { get; set; }

        [Required]
        public string Longitude { get; set; }


        public CorrectionStatus Status { get; set; } = CorrectionStatus.Pending;
        public string? ReviewComment { get; set; }
        public string? ReviewedBy { get; set; }
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedDate { get; set; }
        public string SubmittedBy { get; set; }

        public string? AssignedTo { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public DateTime? DueDate { get; set; }
        public AssignmentPriority Priority { get; set; } = AssignmentPriority.Medium;
        public AssignmentStatus AssignmentStatus { get; set; } = AssignmentStatus.Unassigned;
        public string? AssignmentNotes { get; set; }
    }

    public enum CorrectionStatus
    {
        Pending,
        Approved,
        Rejected
    }

}
