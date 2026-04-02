namespace PianoLearningTracker.Models
{
    // Predstavlja učenika koji pohađa nastavu klavira
    public class Student
    {
        // jedinstveni identifikator učenika
        public int Id { get; set; }

        // ime učenika
        public string FirstName { get; set; }

        // prezime učenika
        public string LastName { get; set; }

        // datum rođenja — koristi se za izračun dobi
        public DateTime DateOfBirth { get; set; }

        // email adresa za kontakt
        public string Email { get; set; }

        // broj telefona za kontakt
        public string PhoneNumber { get; set; }

        // datum upisa u školu/tečaj
        public DateTime EnrollmentDate { get; set; }

        // razred učenika u glazbenoj školi (1-6)
        public int Grade { get; set; }

        // slobodne bilješke o učeniku
        public string Notes { get; set; }

        // 1-N: jedan Student ima više Lessons
        public List<Lesson> Lessons { get; set; }

        // 1-N: jedan Student ima više PracticeSessions
        public List<PracticeSession> PracticeSessions { get; set; }

        // N-N: Student ↔ Piece (bridge tablica StudentPiece)
        public List<StudentPiece> StudentPieces { get; set; }

        public Student()
        {
            Lessons = new List<Lesson>();
            PracticeSessions = new List<PracticeSession>();
            StudentPieces = new List<StudentPiece>();
        }
    }
}
