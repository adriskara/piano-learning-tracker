using System.ComponentModel.DataAnnotations;

namespace PianoLearningTracker.Models
{
    public class PieceFormModel
    {
        [Required(ErrorMessage = "Naziv je obavezan")]
        [StringLength(200, ErrorMessage = "Naziv ne smije biti dulje od 200 znakova")]
        [Display(Name = "Naziv")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Skladatelj je obavezan")]
        [StringLength(200, ErrorMessage = "Ime skladatelja ne smije biti dulje od 200 znakova")]
        [Display(Name = "Skladatelj")]
        public string Composer { get; set; } = null!;

        [Required(ErrorMessage = "Težina je obavezna")]
        [Display(Name = "Težina")]
        public DifficultyLevel Difficulty { get; set; }

        [Display(Name = "Žanr")]
        public string? Genre { get; set; }

        [Required(ErrorMessage = "Trajanje je obavezno")]
        [Range(1, 120, ErrorMessage = "Trajanje mora biti između 1 i 120 minuta")]
        [Display(Name = "Trajanje (min)")]
        public int DurationMinutes { get; set; }

        [Required(ErrorMessage = "Godina nastanka je obavezna")]
        [Range(1000, 2100, ErrorMessage = "Godina nastanka mora biti između 1000 i 2100")]
        [Display(Name = "Godina nastanka")]
        public int YearComposed { get; set; }

        [Display(Name = "Opis")]
        public string? Description { get; set; }
    }
}
