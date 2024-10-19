using System.ComponentModel.DataAnnotations;

namespace KartverketApplikasjon.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Brukernavn")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} må være minst {2} og maks {1} tegn langt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekreft passord")]
        [Compare("Password", ErrorMessage = "Passordene stemmer ikke overens.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Rolle")]
        public UserRole Role { get; set; }
    }
}