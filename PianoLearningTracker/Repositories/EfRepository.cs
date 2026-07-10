using Microsoft.EntityFrameworkCore;
using PianoLearningTracker.DAL;
using PianoLearningTracker.Models;

namespace PianoLearningTracker.Repositories
{
    public class EfRepository : IRepository
    {
        private readonly PianoLearningTrackerDbContext _context;

        public EfRepository(PianoLearningTrackerDbContext context)
        {
            _context = context;
        }

        // ===================== Read =====================

        // Lista (Index/Search): samo Teacher — kartica ne prikazuje ostale relacije
        public IReadOnlyList<Student> GetStudentsForList(int? teacherId = null) =>
            _context.Students
                .Where(s => teacherId == null || s.TeacherId == teacherId)
                .Include(s => s.Teacher)
                .ToList();

        // Dashboard: StudentPieces(.Piece) + PracticeSessions za statistike i achievemente
        public IReadOnlyList<Student> GetStudentsWithProgress(int? teacherId = null) =>
            _context.Students
                .Where(s => teacherId == null || s.TeacherId == teacherId)
                .Include(s => s.PracticeSessions)
                .Include(s => s.StudentPieces).ThenInclude(sp => sp.Piece)
                .ToList();

        public Student? GetStudentById(int id) =>
            _context.Students
                .Include(s => s.Teacher)
                .Include(s => s.Lessons).ThenInclude(l => l.Teacher)
                .Include(s => s.PracticeSessions).ThenInclude(ps => ps.Piece)
                .Include(s => s.StudentPieces).ThenInclude(sp => sp.Piece)
                .FirstOrDefault(s => s.Id == id);

        public IReadOnlyList<Teacher> GetAllTeachers() =>
            _context.Teachers
                .Include(t => t.Lessons)
                .Include(t => t.Students)
                .ToList();

        public Teacher? GetTeacherById(int id) =>
            _context.Teachers
                .Include(t => t.Lessons).ThenInclude(l => l.Student)
                .Include(t => t.Students)
                .FirstOrDefault(t => t.Id == id);

        public Teacher? GetStudentTeacher(int studentId) =>
            _context.Students
                .Where(s => s.Id == studentId)
                .Select(s => s.Teacher)
                .FirstOrDefault();

        public IReadOnlyList<Piece> GetAllPieces() =>
            _context.Pieces.ToList();

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

        public IReadOnlyList<Lesson> GetLessonsByTeacherId(int teacherId) =>
            _context.Lessons
                .Where(l => l.TeacherId == teacherId)
                .Include(l => l.Student)
                .Include(l => l.Teacher)
                .ToList();

        public IReadOnlyList<Lesson> GetLessonsByStudentId(int studentId) =>
            _context.Lessons
                .Where(l => l.StudentId == studentId)
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

        public IReadOnlyList<PracticeSession> GetPracticeSessionsByStudentId(int studentId) =>
            _context.PracticeSessions
                .Where(ps => ps.StudentId == studentId)
                .Include(ps => ps.Student)
                .Include(ps => ps.Piece)
                .ToList();

        public IReadOnlyList<PracticeSession> GetPracticeSessionsByTeacherId(int teacherId) =>
            _context.PracticeSessions
                .Where(ps => ps.Student.TeacherId == teacherId)
                .Include(ps => ps.Student)
                .Include(ps => ps.Piece)
                .ToList();

        public PracticeSession? GetPracticeSessionById(int id) =>
            _context.PracticeSessions
                .Include(ps => ps.Student)
                .Include(ps => ps.Piece)
                .FirstOrDefault(ps => ps.Id == id);

        // ===================== Count =====================

        public int CountStudents() => _context.Students.Count();
        public int CountTeachers() => _context.Teachers.Count();
        public int CountPieces() => _context.Pieces.Count();
        public int CountLessons() => _context.Lessons.Count();

        // ===================== Search =====================

        public IReadOnlyList<Student> SearchStudents(string query, int? teacherId = null) =>
            _context.Students
                .Include(s => s.Teacher)
                .Where(s => (teacherId == null || s.TeacherId == teacherId) &&
                            (string.IsNullOrEmpty(query) ||
                             (s.FirstName + " " + s.LastName).Contains(query) ||
                             s.Email.Contains(query)))
                .ToList();

        public IReadOnlyList<Teacher> SearchTeachers(string query) =>
            _context.Teachers
                .Include(t => t.Lessons)
                .Include(t => t.Students)
                .Where(t => string.IsNullOrEmpty(query) ||
                            (t.FirstName + " " + t.LastName).Contains(query) ||
                            t.Email.Contains(query) ||
                            (t.Specialization != null && t.Specialization.Contains(query)))
                .ToList();

        public IReadOnlyList<Piece> SearchPieces(string query) =>
            _context.Pieces
                .Where(p => string.IsNullOrEmpty(query) ||
                            p.Title.Contains(query) ||
                            p.Composer.Contains(query) ||
                            (p.Genre != null && p.Genre.Contains(query)))
                .ToList();

        public IReadOnlyList<Lesson> SearchLessons(string query, int? teacherId = null, int? studentId = null) =>
            _context.Lessons
                .Include(l => l.Student)
                .Include(l => l.Teacher)
                .Where(l => (teacherId == null || l.TeacherId == teacherId) &&
                            (studentId == null || l.StudentId == studentId) &&
                            (string.IsNullOrEmpty(query) ||
                             (l.Student.FirstName + " " + l.Student.LastName).Contains(query) ||
                             (l.Teacher.FirstName + " " + l.Teacher.LastName).Contains(query) ||
                             (l.Notes != null && l.Notes.Contains(query))))
                .ToList();

        public IReadOnlyList<PracticeSession> SearchPracticeSessions(string query, int? studentId = null, int? teacherId = null) =>
            _context.PracticeSessions
                .Include(ps => ps.Student)
                .Include(ps => ps.Piece)
                .Where(ps => (studentId == null || ps.StudentId == studentId) &&
                             (teacherId == null || ps.Student.TeacherId == teacherId) &&
                             (string.IsNullOrEmpty(query) ||
                              (ps.Student.FirstName + " " + ps.Student.LastName).Contains(query) ||
                              ps.Piece.Title.Contains(query) ||
                              (ps.Notes != null && ps.Notes.Contains(query))))
                .ToList();

        // ===================== Autocomplete =====================

        public IReadOnlyList<AutocompleteItem> AutocompleteTeachers(string query) =>
            _context.Teachers
                .Where(t => string.IsNullOrEmpty(query) ||
                            (t.FirstName + " " + t.LastName).Contains(query))
                .Take(10)
                .Select(t => new AutocompleteItem(t.Id, t.FirstName + " " + t.LastName))
                .ToList();

        public IReadOnlyList<AutocompleteItem> AutocompleteStudents(string query, int? teacherId = null) =>
            _context.Students
                .Where(s => (teacherId == null || s.TeacherId == teacherId) &&
                            (string.IsNullOrEmpty(query) ||
                             (s.FirstName + " " + s.LastName).Contains(query)))
                .Take(10)
                .Select(s => new AutocompleteItem(s.Id, s.FirstName + " " + s.LastName))
                .ToList();

        public IReadOnlyList<AutocompleteItem> AutocompletePieces(string query) =>
            _context.Pieces
                .Where(p => string.IsNullOrEmpty(query) ||
                            p.Title.Contains(query) ||
                            p.Composer.Contains(query))
                .Take(10)
                .Select(p => new AutocompleteItem(p.Id, p.Title + " — " + p.Composer))
                .ToList();

        // ===================== Student CRUD =====================

        public void AddStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public void UpdateStudent(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
        }

        public void DeleteStudent(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }

        // ===================== Teacher CRUD =====================

        public void AddTeacher(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            _context.SaveChanges();
        }

        public void UpdateTeacher(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            _context.SaveChanges();
        }

        public void DeleteTeacher(int id)
        {
            var teacher = _context.Teachers.Find(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                _context.SaveChanges();
            }
        }

        // ===================== Piece CRUD =====================

        public void AddPiece(Piece piece)
        {
            _context.Pieces.Add(piece);
            _context.SaveChanges();
        }

        public void UpdatePiece(Piece piece)
        {
            _context.Pieces.Update(piece);
            _context.SaveChanges();
        }

        public void DeletePiece(int id)
        {
            var piece = _context.Pieces.Find(id);
            if (piece != null)
            {
                _context.Pieces.Remove(piece);
                _context.SaveChanges();
            }
        }

        // ===================== Lesson CRUD =====================

        public void AddLesson(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            _context.SaveChanges();
        }

        public void UpdateLesson(Lesson lesson)
        {
            _context.Lessons.Update(lesson);
            _context.SaveChanges();
        }

        public void DeleteLesson(int id)
        {
            var lesson = _context.Lessons.Find(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
                _context.SaveChanges();
            }
        }

        // ===================== PracticeSession CRUD =====================

        public void AddPracticeSession(PracticeSession session)
        {
            _context.PracticeSessions.Add(session);
            _context.SaveChanges();
        }

        public void UpdatePracticeSession(PracticeSession session)
        {
            _context.PracticeSessions.Update(session);
            _context.SaveChanges();
        }

        public void DeletePracticeSession(int id)
        {
            var session = _context.PracticeSessions.Find(id);
            if (session != null)
            {
                _context.PracticeSessions.Remove(session);
                _context.SaveChanges();
            }
        }
    }
}
