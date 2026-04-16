using PianoLearningTracker.Models;

namespace PianoLearningTracker.Repositories
{
    public interface IMockRepository
    {
        IReadOnlyList<Student> GetAllStudents();
        Student? GetStudentById(int id);

        IReadOnlyList<Teacher> GetAllTeachers();
        Teacher? GetTeacherById(int id);

        IReadOnlyList<Piece> GetAllPieces();
        Piece? GetPieceById(int id);

        IReadOnlyList<Lesson> GetAllLessons();
        Lesson? GetLessonById(int id);

        IReadOnlyList<PracticeSession> GetAllPracticeSessions();
        PracticeSession? GetPracticeSessionById(int id);
    }
}
