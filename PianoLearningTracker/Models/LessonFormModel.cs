using System.ComponentModel.DataAnnotations;

namespace PianoLearningTracker.Models
{
    public class LessonFormModel
    {
        [Required(ErrorMessage = "Datum je obavezan")]
        [Display(Name = "Datum i vrijeme")]
        public DateTime ScheduledDate { get; set; }

        [Required(ErrorMessage = "Trajanje je obavezno")]
        [Range(15, 180, ErrorMessage = "Trajanje mora biti između 15 i 180 minuta")]
        [Display(Name = "Trajanje (min)")]
        public int DurationMinutes { get; set; }

        [Required(ErrorMessage = "Status je obavezan")]
        [Display(Name = "Status")]
        public LessonStatus Status { get; set; }

        [Display(Name = "Bilješke")]
        public string? Notes { get; set; }

        [Display(Name = "Domaća zadaća")]
        public string? HomeworkAssigned { get; set; }

        [Required(ErrorMessage = "Učenik je obavezan")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite učenika")]
        [Display(Name = "Učenik")]
        public int StudentId { get; set; }
        public string? StudentName { get; set; }

        [Required(ErrorMessage = "Profesor je obavezan")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite profesora")]
        [Display(Name = "Profesor")]
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
    }
}
