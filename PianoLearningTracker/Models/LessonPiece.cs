using System.ComponentModel.DataAnnotations.Schema;

namespace PianoLearningTracker.Models
{
    // Bridge tablica za N-N vezu Lesson ↔ Piece:
    // na jednom satu može se raditi više skladbi, a jedna skladba može biti obrađena na više satova
    public class LessonPiece
    {
        // strani ključ — koji sat (dio složenog primarnog ključa, konfiguriran u DbContext)
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }

        // strani ključ — koja skladba je obrađena na tom satu (dio složenog primarnog ključa)
        [ForeignKey("Piece")]
        public int PieceId { get; set; }

        // bilješke vezane za obradu ove skladbe na ovom satu
        public string? Notes { get; set; }

        // navigacijsko svojstvo — cijeli objekt sata
        public virtual Lesson Lesson { get; set; } = null!;

        // navigacijsko svojstvo — cijeli objekt skladbe
        public virtual Piece Piece { get; set; } = null!;
    }
}
