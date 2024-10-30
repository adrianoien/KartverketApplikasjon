using System.ComponentModel.DataAnnotations;

namespace KartverketApplikasjon.Models
{
    public class UserData
    {
        [Key] 
        public int Id { get; set; } 
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        User,
        Saksbehandler
    }
}