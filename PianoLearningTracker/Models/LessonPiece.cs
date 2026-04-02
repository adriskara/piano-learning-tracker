namespace PianoLearningTracker.Models
{
    // Bridge tablica za N-N vezu Lesson ↔ Piece:
    // na jednom satu može se raditi više skladbi, a jedna skladba može biti obrađena na više satova
    public class LessonPiece
    {
        // strani ključ — koji sat
        public int LessonId { get; set; }

        // strani ključ — koja skladba je obrađena na tom satu
        public int PieceId { get; set; }

        // bilješke vezane za obradu ove skladbe na ovom satu
        public string Notes { get; set; }

        // navigacijsko svojstvo — cijeli objekt sata
        public Lesson Lesson { get; set; }

        // navigacijsko svojstvo — cijeli objekt skladbe
        public Piece Piece { get; set; }
    }
}
