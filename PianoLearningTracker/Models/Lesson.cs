using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PianoLearningTracker.Models
{
    // Predstavlja jedan sat klavira između nastavnika i učenika
    public class Lesson
    {
        // jedinstveni identifikator sata
        [Key]
        public int Id { get; set; }

        // datum i vrijeme kada je sat zakazan
        public DateTime ScheduledDate { get; set; }

        // trajanje sata u minutama
        public int DurationMinutes { get; set; }

        // status sata (enum: Scheduled, Completed, Cancelled, Missed)
        public LessonStatus Status { get; set; }

        // bilješke nastavnika s ovog sata
        public string? Notes { get; set; }

        // domaća zadaća zadana učeniku nakon sata
        public string? HomeworkAssigned { get; set; }

        // strani ključ — koji učenik pohađa ovaj sat
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        // strani ključ — koji nastavnik drži ovaj sat
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        // navigacijsko svojstvo — cijeli objekt učenika
        public virtual Student Student { get; set; } = null!;

        // navigacijsko svojstvo — cijeli objekt nastavnika
        public virtual Teacher Teacher { get; set; } = null!;

        // N-N: Lesson ↔ Piece (bridge tablica LessonPiece)
        public virtual ICollection<LessonPiece> LessonPieces { get; set; } = new List<LessonPiece>();
    }
}
