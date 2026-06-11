using System.ComponentModel.DataAnnotations;

namespace PianoLearningTracker.Models
{
    // Predstavlja učenika koji pohađa nastavu klavira
    public class Student
    {
        // jedinstveni identifikator učenika
        [Key]
        public int Id { get; set; }

        // ime učenika
        public string FirstName { get; set; } = null!;

        // prezime učenika
        public string LastName { get; set; } = null!;

        // datum rođenja — koristi se za izračun dobi
        public DateTime DateOfBirth { get; set; }

        // email adresa za kontakt
        public string Email { get; set; } = null!;

        // broj telefona za kontakt
        public string PhoneNumber { get; set; } = null!;

        // datum upisa u školu/tečaj
        public DateTime EnrollmentDate { get; set; }

        // razred učenika u glazbenoj školi (1-6)
        public int Grade { get; set; }

        // slobodne bilješke o učeniku
        public string? Notes { get; set; }

        // putanja do profilne slike učenika
        public string? ProfileImagePath { get; set; }

        // FK na primarnog profesora
        public int? TeacherId { get; set; }
        public virtual Teacher? Teacher { get; set; }

        // 1-N: jedan Student ima više Lessons
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

        // 1-N: jedan Student ima više PracticeSessions
        public virtual ICollection<PracticeSession> PracticeSessions { get; set; } = new List<PracticeSession>();

        // N-N: Student ↔ Piece (bridge tablica StudentPiece)
        public virtual ICollection<StudentPiece> StudentPieces { get; set; } = new List<StudentPiece>();
    }
}
