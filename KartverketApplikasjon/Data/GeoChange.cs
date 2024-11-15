using System.ComponentModel.DataAnnotations;
using KartverketApplikasjon.Models;

namespace KartverketApplikasjon.Data
{
 // Represents a geographical change submitted by a user, including details such as GeoJSON data, description, status, review information, assignment details, and priority.
    public class GeoChange
    {
        public int Id { get; set; }
        public string GeoJson { get; set; }
        public string Description { get; set; }
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
}