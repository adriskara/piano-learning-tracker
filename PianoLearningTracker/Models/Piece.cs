using System.ComponentModel.DataAnnotations;

namespace PianoLearningTracker.Models
{
    // Predstavlja glazbenu skladbu koja se uči na satu ili vježba
    public class Piece
    {
        // jedinstveni identifikator skladbe
        [Key]
        public int Id { get; set; }

        // naziv skladbe (npr. "Für Elise")
        public string Title { get; set; } = null!;

        // ime skladatelja (npr. "Ludwig van Beethoven")
        public string Composer { get; set; } = null!;

        // težina skladbe (enum: Beginner → Expert)
        public DifficultyLevel Difficulty { get; set; }

        // glazbeni žanr (npr. "Klasika", "Etida", "Sonata")
        public string? Genre { get; set; }

        // trajanje izvođenja u minutama
        public int DurationMinutes { get; set; }

        // godina nastanka skladbe
        public int YearComposed { get; set; }

        // kratki opis ili napomene o skladbi
        public string? Description { get; set; }

        // N-N: Piece ↔ Student (bridge tablica StudentPiece)
        public virtual ICollection<StudentPiece> StudentPieces { get; set; } = new List<StudentPiece>();

        // N-N: Piece ↔ Lesson (bridge tablica LessonPiece)
        public virtual ICollection<LessonPiece> LessonPieces { get; set; } = new List<LessonPiece>();

        // 1-N: jedna Piece ima više PracticeSessions
        public virtual ICollection<PracticeSession> PracticeSessions { get; set; } = new List<PracticeSession>();

        // 1-N: jedna Piece ima više Attachments (note, audio...)
        public virtual ICollection<PieceAttachment> Attachments { get; set; } = new List<PieceAttachment>();
    }
}
