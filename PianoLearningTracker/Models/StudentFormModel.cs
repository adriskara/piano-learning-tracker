using System.ComponentModel.DataAnnotations;

namespace PianoLearningTracker.Models
{
    public class StudentFormModel
    {
        [Required(ErrorMessage = "Ime je obavezno")]
        [StringLength(50, ErrorMessage = "Ime ne smije biti dulje od 50 znakova")]
        [Display(Name = "Ime")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Prezime je obavezno")]
        [StringLength(50, ErrorMessage = "Prezime ne smije biti dulje od 50 znakova")]
        [Display(Name = "Prezime")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Datum rođenja je obavezan")]
        [Display(Name = "Datum rođenja")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Telefon je obavezan")]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Datum upisa je obavezan")]
        [Display(Name = "Datum upisa")]
        public DateTime EnrollmentDate { get; set; }

        [Required(ErrorMessage = "Razred je obavezan")]
        [Range(1, 6, ErrorMessage = "Razred mora biti između 1 i 6")]
        [Display(Name = "Razred")]
        public int Grade { get; set; }

        [Display(Name = "Bilješke")]
        public string? Notes { get; set; }

        [Display(Name = "Profesor")]
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }

        [Display(Name = "Profilna slika")]
        public string? ProfileImagePath { get; set; }
    }
}
