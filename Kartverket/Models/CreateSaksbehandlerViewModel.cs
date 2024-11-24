using System.ComponentModel.DataAnnotations;

namespace Kartverket.Models
{
    public class CreateSaksbehandlerViewModel : IValidatableObject
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Email.EndsWith("@Kartverket.no", StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult("Email must be from the domain @Kartverket.no", new[] { nameof(Email) });
            }
        }
    }
}
