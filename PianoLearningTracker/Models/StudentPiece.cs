using System.ComponentModel.DataAnnotations.Schema;

namespace PianoLearningTracker.Models
{
    // Bridge tablica za N-N vezu Student ↔ Piece:
    // jedan student može učiti više skladbi, a jedna skladba može biti dodijeljena više studenata
    public class StudentPiece
    {
        // strani ključ prema tablici Student (dio složenog primarnog ključa, konfiguriran u DbContext)
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        // strani ključ prema tablici Piece (dio složenog primarnog ključa)
        [ForeignKey("Piece")]
        public int PieceId { get; set; }

        // datum kada je student počeo učiti ovu skladbu
        public DateTime StartDate { get; set; }

        // je li student završio učenje ove skladbe
        public bool IsCompleted { get; set; }

        // datum završetka — nullable jer možda još nije završeno
        public DateTime? CompletionDate { get; set; }

        // ocjena napretka na ovoj skladbi (1-10)
        public int ProgressRating { get; set; }

        // dodatne bilješke vezane za ovaj par student-skladba
        public string? Notes { get; set; }

        // navigacijsko svojstvo — omogućuje pristup cijelom Student objektu
        public virtual Student Student { get; set; } = null!;

        // navigacijsko svojstvo — omogućuje pristup cijelom Piece objektu
        public virtual Piece Piece { get; set; } = null!;
    }
}
