using Microsoft.EntityFrameworkCore;
using PianoLearningTracker.DAL;
using PianoLearningTracker.Models;

namespace PianoLearningTracker.Repositories
{
    public class EfRepository : IMockRepository
    {
        private readonly PianoLearningTrackerDbContext _context;

        public EfRepository(PianoLearningTrackerDbContext context)
        {
            _context = context;
        }

        public IReadOnlyList<Student> GetAllStudents() =>
            _context.Students
                .Include(s => s.Lessons)
                .Include(s => s.PracticeSessions)
                .Include(s => s.StudentPieces).ThenInclude(sp => sp.Piece)
                .ToList();

        public Student? GetStudentById(int id) =>
            _context.Students
                .Include(s => s.Lessons).ThenInclude(l => l.Teacher)
                .Include(s => s.PracticeSessions).ThenInclude(ps => ps.Piece)
                .Include(s => s.StudentPieces).ThenInclude(sp => sp.Piece)
                .FirstOrDefault(s => s.Id == id);

        public IReadOnlyList<Teacher> GetAllTeachers() =>
            _context.Teachers
                .Include(t => t.Lessons)
                .ToList();

        public Teacher? GetTeacherById(int id) =>
            _context.Teachers
                .Include(t => t.Lessons).ThenInclude(l => l.Student)
                .FirstOrDefault(t => t.Id == id);

        public IReadOnlyList<Piece> GetAllPieces() =>
            _context.Pieces
                .ToList();

        public Piece? GetPieceById(int id) =>
            _context.Pieces
                .Include(p => p.StudentPieces).ThenInclude(sp => sp.Student)
                .Include(p => p.LessonPieces).ThenInclude(lp => lp.Lesson)
                .Include(p => p.PracticeSessions)
                .FirstOrDefault(p => p.Id == id);

        public IReadOnlyList<Lesson> GetAllLessons() =>
            _context.Lessons
                .Include(l => l.Student)
                .Include(l => l.Teacher)
                .ToList();

        public Lesson? GetLessonById(int id) =>
            _context.Lessons
                .Include(l => l.Student)
                .Include(l => l.Teacher)
                .Include(l => l.LessonPieces).ThenInclude(lp => lp.Piece)
                .FirstOrDefault(l => l.Id == id);

        public IReadOnlyList<PracticeSession> GetAllPracticeSessions() =>
            _context.PracticeSessions
                .Include(ps => ps.Student)
                .Include(ps => ps.Piece)
                .ToList();

        public PracticeSession? GetPracticeSessionById(int id) =>
            _context.PracticeSessions
                .Include(ps => ps.Student)
                .Include(ps => ps.Piece)
                .FirstOrDefault(ps => ps.Id == id);
    }
}
