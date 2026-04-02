namespace PianoLearningTracker.Models
{
    // Predstavlja nastavnika koji drži nastavu klavira
    public class Teacher
    {
        // jedinstveni identifikator nastavnika
        public int Id { get; set; }

        // ime nastavnika
        public string FirstName { get; set; }

        // prezime nastavnika
        public string LastName { get; set; }

        // email adresa za kontakt
        public string Email { get; set; }

        // broj telefona za kontakt
        public string PhoneNumber { get; set; }

        // područje specijalizacije (npr. "Klasična glazba", "Jazz")
        public string Specialization { get; set; }

        // broj godina radnog iskustva u podučavanju
        public int YearsOfExperience { get; set; }

        // datum zapošljavanja u školi
        public DateTime HireDate { get; set; }

        // kratka biografija nastavnika
        public string Biography { get; set; }

        // 1-N: jedan Teacher ima više Lessons
        public List<Lesson> Lessons { get; set; }

        public Teacher()
        {
            Lessons = new List<Lesson>();
        }
    }
}
