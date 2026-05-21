using System.ComponentModel.DataAnnotations;

namespace PianoLearningTracker.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Unesite valjanu email adresu.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 znakova.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrda lozinke je obavezna.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Lozinke se ne podudaraju.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Uloga je obavezna.")]
        public string Role { get; set; } = string.Empty;

        // Zajednička polja za Teacher i Student
        [Display(Name = "Ime")]
        public string? FirstName { get; set; }

        [Display(Name = "Prezime")]
        public string? LastName { get; set; }

        [Display(Name = "Broj telefona")]
        public string? PhoneNumber { get; set; }

        // Teacher-specific
        [Display(Name = "Specijalizacija")]
        public string? Specialization { get; set; }

        [Display(Name = "Godine iskustva")]
        [Range(0, 60)]
        public int? YearsOfExperience { get; set; }

        // Student-specific
        [Display(Name = "Datum rođenja")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Razred (1-6)")]
        [Range(1, 6)]
        public int? Grade { get; set; }

        [Display(Name = "Primarni profesor")]
        public int? TeacherId { get; set; }
    }
}
