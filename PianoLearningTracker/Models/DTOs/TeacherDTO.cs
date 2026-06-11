namespace PianoLearningTracker.Models.DTOs
{
    public class TeacherBriefDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class TeacherDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Specialization { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime HireDate { get; set; }
        public string? Biography { get; set; }
    }
}
