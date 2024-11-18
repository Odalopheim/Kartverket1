using System.ComponentModel.DataAnnotations;

namespace Kartverket.ViewModels
{
    public class RegistrerViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Navn")]
        public string Name { get; set; }

        [Required]
        [StringLength(4)]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Postnummeret må være 4 siffer.")]
        [Display(Name = "Postnummer")]
        public string PostNumber { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Bekreft Passord")]
        public string ConfirmPassword { get; set; }
    }
}
