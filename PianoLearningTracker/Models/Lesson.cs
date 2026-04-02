namespace PianoLearningTracker.Models
{
    // Predstavlja jedan sat klavira između nastavnika i učenika
    public class Lesson
    {
        // jedinstveni identifikator sata
        public int Id { get; set; }

        // datum i vrijeme kada je sat zakazan
        public DateTime ScheduledDate { get; set; }

        // trajanje sata u minutama
        public int DurationMinutes { get; set; }

        // status sata (enum: Scheduled, Completed, Cancelled, Missed)
        public LessonStatus Status { get; set; }

        // bilješke nastavnika s ovog sata
        public string Notes { get; set; }

        // domaća zadaća zadana učeniku nakon sata
        public string HomeworkAssigned { get; set; }

        // strani ključ — koji učenik pohađa ovaj sat
        public int StudentId { get; set; }

        // strani ključ — koji nastavnik drži ovaj sat
        public int TeacherId { get; set; }

        // navigacijsko svojstvo — cijeli objekt učenika
        public Student Student { get; set; }

        // navigacijsko svojstvo — cijeli objekt nastavnika
        public Teacher Teacher { get; set; }

        // N-N: Lesson ↔ Piece (bridge tablica LessonPiece)
        public List<LessonPiece> LessonPieces { get; set; }

        public Lesson()
        {
            LessonPieces = new List<LessonPiece>();
        }
    }
}
