using System.ComponentModel.DataAnnotations;

namespace KartverketApplikasjon.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [Display(Name = "Husk meg")]
        public bool RememberMe { get; set; }
    }
}