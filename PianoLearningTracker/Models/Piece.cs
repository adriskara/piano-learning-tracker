namespace PianoLearningTracker.Models
{
    // Predstavlja glazbenu skladbu koja se uči na satu ili vježba
    public class Piece
    {
        // jedinstveni identifikator skladbe
        public int Id { get; set; }

        // naziv skladbe (npr. "Für Elise")
        public string Title { get; set; }

        // ime skladatelja (npr. "Ludwig van Beethoven")
        public string Composer { get; set; }

        // težina skladbe (enum: Beginner → Expert)
        public DifficultyLevel Difficulty { get; set; }

        // glazbeni žanr (npr. "Klasika", "Etida", "Sonata")
        public string Genre { get; set; }

        // trajanje izvođenja u minutama
        public int DurationMinutes { get; set; }

        // godina nastanka skladbe
        public int YearComposed { get; set; }

        // kratki opis ili napomene o skladbi
        public string Description { get; set; }

        // N-N: Piece ↔ Student (bridge tablica StudentPiece)
        public List<StudentPiece> StudentPieces { get; set; }

        // N-N: Piece ↔ Lesson (bridge tablica LessonPiece)
        public List<LessonPiece> LessonPieces { get; set; }

        // 1-N: jedna Piece ima više PracticeSessions
        public List<PracticeSession> PracticeSessions { get; set; }

        public Piece()
        {
            StudentPieces = new List<StudentPiece>();
            LessonPieces = new List<LessonPiece>();
            PracticeSessions = new List<PracticeSession>();
        }
    }
}
