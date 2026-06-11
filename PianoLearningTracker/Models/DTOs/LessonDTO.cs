namespace PianoLearningTracker.Models.DTOs
{
    public class LessonDTO
    {
        public int Id { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public string? HomeworkAssigned { get; set; }
        public int StudentId { get; set; }
        public StudentBriefDTO Student { get; set; } = null!;
        public int TeacherId { get; set; }
        public TeacherBriefDTO Teacher { get; set; } = null!;
    }
}
