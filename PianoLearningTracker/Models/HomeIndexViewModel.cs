namespace PianoLearningTracker.Models
{
    public class HomeIndexViewModel
    {
        public int StudentCount { get; set; }
        public int TeacherCount { get; set; }
        public int PieceCount { get; set; }
        public int LessonCount { get; set; }
        public int PracticeSessionCount { get; set; }
        public int ScheduledLessonCount { get; set; }

        public Lesson? NextLesson { get; set; }
        public PracticeSession? LatestPracticeSession { get; set; }
    }
}
