namespace PianoLearningTracker.Models
{
    public class PracticeStreakItem
    {
        public string StudentName { get; set; } = "";
        public int SessionCount { get; set; }
        public int TotalMinutes { get; set; }
    }

    public class HomeIndexViewModel
    {
        public string UserRole { get; set; } = "Administrator";
        public string? UserDisplayName { get; set; }
        public string UserFirstName { get; set; } = "";

        // Statistike (Admin: ukupno; Teacher/Student: filtrirano)
        public int StudentCount { get; set; }
        public int TeacherCount { get; set; }
        public int PieceCount { get; set; }
        public int LessonCount { get; set; }
        public int PracticeSessionCount { get; set; }
        public int ScheduledLessonCount { get; set; }
        public int CompletedPieceCount { get; set; }

        public Lesson? NextLesson { get; set; }
        public PracticeSession? LatestPracticeSession { get; set; }
        public List<PracticeStreakItem> PracticeStreaks { get; set; } = new();
        public List<StudentPiece> RecentAchievements { get; set; } = new();

        public List<Lesson> HomeworkLessons { get; set; } = new();
        public List<Lesson> CalendarLessons { get; set; } = new();
    }

    public class LandingViewModel
    {
        public int StudentCount { get; set; }
        public int TeacherCount { get; set; }
        public int LessonCount { get; set; }
        public int PieceCount { get; set; }
    }
}
