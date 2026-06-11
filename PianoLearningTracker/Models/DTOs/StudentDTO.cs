namespace PianoLearningTracker.Models.DTOs
{
    public class StudentBriefDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class StudentDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
        public int Grade { get; set; }
        public string? Notes { get; set; }
        public int? TeacherId { get; set; }
        public TeacherBriefDTO? Teacher { get; set; }
    }
}
