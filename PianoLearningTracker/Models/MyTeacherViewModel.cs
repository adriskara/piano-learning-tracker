namespace PianoLearningTracker.Models
{
    // Sigurni prikaz profesora za učenika — sadrži samo podatke o profesoru,
    // bez popisa drugih učenika ili njihove aktivnosti.
    public class MyTeacherViewModel
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? Specialization { get; set; }
        public int YearsOfExperience { get; set; }
        public string? Biography { get; set; }
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }
    }
}
