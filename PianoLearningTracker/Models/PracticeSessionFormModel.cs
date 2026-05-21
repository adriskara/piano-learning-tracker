using System.ComponentModel.DataAnnotations;

namespace PianoLearningTracker.Models
{
    public class PracticeSessionFormModel
    {
        [Required(ErrorMessage = "Datum je obavezan")]
        [Display(Name = "Datum i vrijeme")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Trajanje je obavezno")]
        [Range(1, 480, ErrorMessage = "Trajanje mora biti između 1 i 480 minuta")]
        [Display(Name = "Trajanje (min)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Bilješke")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Ocjena je obavezna")]
        [Range(1, 5, ErrorMessage = "Ocjena mora biti između 1 i 5")]
        [Display(Name = "Ocjena kvalitete (1–5)")]
        public int QualityRating { get; set; }

        [Display(Name = "Ciljevi")]
        public string? Goals { get; set; }

        [Required(ErrorMessage = "Učenik je obavezan")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite učenika")]
        [Display(Name = "Učenik")]
        public int StudentId { get; set; }
        public string? StudentName { get; set; }

        [Required(ErrorMessage = "Skladba je obavezna")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite skladbu")]
        [Display(Name = "Skladba")]
        public int PieceId { get; set; }
        public string? PieceName { get; set; }
    }
}
