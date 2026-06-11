using System.ComponentModel.DataAnnotations;

namespace PianoLearningTracker.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OIB je obavezan.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB mora imati točno 11 znamenki.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "OIB smije sadržavati samo brojeve.")]
        [Display(Name = "OIB")]
        public string OIB { get; set; } = string.Empty;
    }
}
