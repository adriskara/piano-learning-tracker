using System.ComponentModel.DataAnnotations;

namespace PianoLearningTracker.Models
{
    // Predstavlja nastavnika koji drži nastavu klavira
    public class Teacher
    {
        // jedinstveni identifikator nastavnika
        [Key]
        public int Id { get; set; }

        // ime nastavnika
        public string FirstName { get; set; } = null!;

        // prezime nastavnika
        public string LastName { get; set; } = null!;

        // email adresa za kontakt
        public string Email { get; set; } = null!;

        // broj telefona za kontakt
        public string PhoneNumber { get; set; } = null!;

        // područje specijalizacije (npr. "Klasična glazba", "Jazz")
        public string? Specialization { get; set; }

        // broj godina radnog iskustva u podučavanju
        public int YearsOfExperience { get; set; }

        // datum zapošljavanja u školi
        public DateTime HireDate { get; set; }

        // kratka biografija nastavnika
        public string? Biography { get; set; }

        // 1-N: jedan Teacher ima više Lessons
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
