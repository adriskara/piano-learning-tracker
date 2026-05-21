using System.ComponentModel.DataAnnotations;

namespace PianoLearningTracker.Models
{
    public class TeacherFormModel
    {
        [Required(ErrorMessage = "Ime je obavezno")]
        [StringLength(50, ErrorMessage = "Ime ne smije biti dulje od 50 znakova")]
        [Display(Name = "Ime")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Prezime je obavezno")]
        [StringLength(50, ErrorMessage = "Prezime ne smije biti dulje od 50 znakova")]
        [Display(Name = "Prezime")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Telefon je obavezan")]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; } = null!;

        [Display(Name = "Specijalizacija")]
        public string? Specialization { get; set; }

        [Required(ErrorMessage = "Godine iskustva su obavezne")]
        [Range(0, 50, ErrorMessage = "Godine iskustva moraju biti između 0 i 50")]
        [Display(Name = "Godine iskustva")]
        public int YearsOfExperience { get; set; }

        [Required(ErrorMessage = "Datum zaposlenja je obavezan")]
        [Display(Name = "Datum zaposlenja")]
        public DateTime HireDate { get; set; }

        [Display(Name = "Biografija")]
        public string? Biography { get; set; }
    }
}
