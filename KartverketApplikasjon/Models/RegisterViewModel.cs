using System.ComponentModel.DataAnnotations;

namespace KartverketApplikasjon.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Brukernavn er påkrevd")]
        [Display(Name = "Brukernavn")]
        public string Username { get; set; }

        [Required(ErrorMessage = "E-post er påkrevd")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Passord er påkrevd")]
        [StringLength(100, ErrorMessage = "Passordet må være minst {2} tegn langt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekreft passord")]
        [Compare("Password", ErrorMessage = "Passordene stemmer ikke overens.")]
        public string ConfirmPassword { get; set; }

        // You can add these fields from UserData if needed
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}